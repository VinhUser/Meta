﻿using AutoMapper;
using Azure.Core;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.EnumStatus;
using Meta.BusinessTier.Enums.EnumTypes;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
using Meta.BusinessTier.Extensions;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Order;
using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Payload.Task;
using Meta.BusinessTier.Services.Interfaces;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.DataTier.Paginate;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public class OrderService : BaseService<OrderService>, IOrderService
    {
        public OrderService(IUnitOfWork<MetaContext> unitOfWork, ILogger<OrderService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<Guid> CreateNewOrder(CreateNewOrderResponse request)
        {
            var currentUser = GetUsernameFromJwt();
            var pet = await _unitOfWork.GetRepository<Pet>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(request.PetId))
                ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(pet.AccountId))
                ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);

            DateTime currentTime = TimeUtils.GetCurrentSEATime();
            Order newOrder = new()
            {
                Id = Guid.NewGuid(),
                InvoiceCode = TimeUtils.GetTimestamp(currentTime),
                CreatedDate = currentTime,
                CompletedDate = null,
                ExcutionDate = request.ExcutionDate, // Set execution date
                TotalAmount = 0,
                Description = request.Description,
                Status = OrderStatus.UNPAID.GetDescriptionFromEnum(),
                AccountId = account.Id,
                PetId = request.PetId,
                Type = request.Type.ToString() // Assign the type of request
            };

            var orderDetails = new List<OrderDetail>();
            double totalAmount = 0;

            foreach (var product in request.ProductList)
            {
                OrderDetail orderDetail;

                var supProductExists = await _unitOfWork.GetRepository<SupProduct>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(product.ProductId));
                if (supProductExists != null)
                {
                    orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = newOrder.Id,
                        SupProductId = product.ProductId,
                        Quantity = product.Quantity,
                        SellingPrice = product.SellingPrice,
                        TotalAmount = product.SellingPrice * product.Quantity
                    };
                }
                else
                {
                    var productExists = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(product.ProductId));
                    if (productExists == null)
                    {
                        throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);
                    }

                    orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = newOrder.Id,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        SellingPrice = product.SellingPrice,
                        TotalAmount = product.SellingPrice * product.Quantity
                    };
                }

                orderDetails.Add(orderDetail);
                totalAmount += orderDetail.TotalAmount ?? 0;
            }

            var accountRank = await _unitOfWork.GetRepository<AccountRank>()
                .SingleOrDefaultAsync(predicate: ar => ar.AccountId == account.Id);

            if (accountRank != null)
            {
                var rank = await _unitOfWork.GetRepository<Rank>()
                    .SingleOrDefaultAsync(predicate: r => r.Id == accountRank.RankId);

                double rankDiscount = (rank.Value.Value / 100.0) * totalAmount;
                newOrder.FinalAmount = totalAmount - rankDiscount;
            }
            else
            {
                newOrder.FinalAmount = totalAmount;
            }

            // Insert the new order and its details
            await _unitOfWork.GetRepository<Order>().InsertAsync(newOrder);
            await _unitOfWork.GetRepository<OrderDetail>().InsertRangeAsync(orderDetails);

            // Handle task creation based on the request type
            if (request.Type == OrderType.CustomerRequest)
            {
                TaskManager newTask = new TaskManager
                {
                    Id = Guid.NewGuid(),
                    Type = TaskType.CustomerRequest.ToString(),
                    Status = TaskManagerStatus.Process.GetDescriptionFromEnum(),
                    CreateDate = currentTime,
                    ExcutionDate = newOrder.ExcutionDate,
                    OrderId = newOrder.Id,
                    AccountId = request.StaffId // Assuming you have this in the request
                };

                await _unitOfWork.GetRepository<TaskManager>().InsertAsync(newTask);
            }

            await _unitOfWork.CommitAsync();

            return newOrder.Id;
        }



        public async Task<GetOrderDetailResponse> GetOrderDetail(Guid orderId)
        {
            var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(orderId),
                selector: x => new GetOrderDetailResponse
                {
                    OrderId = x.Id,
                    InvoiceCode = x.InvoiceCode,
                    CreatedDate = x.CreatedDate,
                    CompletedDate = x.CompletedDate,
                    Description = x.Description,
                    TotalAmount = x.TotalAmount,
                    FinalAmount = x.FinalAmount,
                    Status = EnumUtil.ParseEnum<OrderStatus>(x.Status),
                    Note = x.Notes.Select(note => new NoteResponse
                    {
                        Id = note.Id,
                        Status = EnumUtil.ParseEnum<NoteStatus>(note.Status),
                        Description = note.Description,
                        CreateDate = note.CreateDate.Value,
                    }).ToList(),
                    UserInfo = x.Pet.Account == null ? null : new OrderUserResponse
                    {
                        Id = x.Pet.Account.Id,
                        FullName = x.Pet.Account.FullName,
                        Role = EnumUtil.ParseEnum<RoleEnum>(x.Pet.Account.Role)
                    },
                    PetInfor = x.Pet == null ? null : new OrderPetResponse
                    {
                        Id = x.Pet.Id,
                        Name = x.Pet.Name,
                    },
                    ProductList = x.OrderDetails.Select(detail => new OrderDetailResponse
                    {
                        OrderDetailId = detail.Id,
                        ProductId = detail.ProductId,
                        ProductName = detail.Product.Name,
                        SupProductId = detail.SupProductId,
                        SupProductName = detail.SupProduct.Name,
                        Quantity = detail.Quantity,
                        SellingPrice = detail.SellingPrice,
                        TotalAmount = detail.TotalAmount,
                    }).ToList()
                },
                include: x => x.Include(x => x.Pet)
                               .ThenInclude(pet => pet.Account)
                               .Include(x => x.OrderDetails)
                                   .ThenInclude(detail => detail.Product)
                               .Include(x => x.OrderDetails)
                                   .ThenInclude(detail => detail.SupProduct)
                               .Include(x => x.Notes)
            );

            if (order == null)
            {
                throw new KeyNotFoundException(MessageConstant.Order.OrderNotFoundMessage);
            }

            return order;
        }

        public async Task<IPaginate<GetOrderDetailResponse>> GetOrderList(OrderFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetOrderDetailResponse> orderList = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
                selector: x => new GetOrderDetailResponse
                {
                    OrderId = x.Id,
                    InvoiceCode = x.InvoiceCode,
                    CreatedDate = x.CreatedDate,
                    CompletedDate = x.CompletedDate,
                    TotalAmount = x.TotalAmount,
                    FinalAmount = x.FinalAmount,
                    Description = x.Description,
                    Status = EnumUtil.ParseEnum<OrderStatus>(x.Status),
                    Note = x.Notes.Select(note => new NoteResponse
                    {
                        Id = note.Id,
                        Status = EnumUtil.ParseEnum<NoteStatus>(note.Status),
                        Description = note.Description,
                        CreateDate = note.CreateDate.Value,
                    }).ToList(),
                    UserInfo = x.Pet.Account == null ? null : new OrderUserResponse
                    {
                        Id = x.Pet.Account.Id,
                        FullName = x.Pet.Account.FullName,
                        Role = EnumUtil.ParseEnum<RoleEnum>(x.Pet.Account.Role)
                    },
                    PetInfor = x.Pet == null ? null : new OrderPetResponse
                    {
                        Id = x.Pet.Id,
                        Name = x.Pet.Name,
                    },
                    ProductList = x.OrderDetails.Select(detail => new OrderDetailResponse
                    {
                        OrderDetailId = detail.Id,
                        ProductId = detail.ProductId,
                        ProductName = detail.Product.Name,
                        SupProductId = detail.SupProductId,
                        SupProductName = detail.SupProduct.Name,
                        Quantity = detail.Quantity,
                        SellingPrice = detail.SellingPrice,
                        TotalAmount = detail.TotalAmount,
                    }).ToList()
                },
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.CreatedDate),
                include: x => x.Include(x => x.Pet.Account)
                              .Include(x => x.Pet)
                              .Include(x => x.OrderDetails)
                                  .ThenInclude(detail => detail.Product)
                              .Include(x => x.OrderDetails)
                                  .ThenInclude(detail => detail.SupProduct)
                              .Include(x => x.Notes),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return orderList;
        }


        public async Task<bool> UpdateOrder(Guid orderId, UpdateOrderRequest request)
        {
            string currentUser = GetUsernameFromJwt();
            var userId = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(currentUser),
                selector: x => x.Id);
            Order updateOrder = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(orderId))
                ?? throw new BadHttpRequestException(MessageConstant.Order.OrderNotFoundMessage);
            DateTime currentTime = TimeUtils.GetCurrentSEATime();
            switch (request.Status)
            {
                case OrderStatus.COMPLETED:
                    break;
                case OrderStatus.UNPAID:
                    break;
                case OrderStatus.PAID:
                    break;
                case OrderStatus.CANCELED:
                    updateOrder.Status = OrderStatus.CANCELED.GetDescriptionFromEnum();
                    _unitOfWork.GetRepository<Order>().UpdateAsync(updateOrder);

                    break;
                default:
                    return false;
            }
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public Task<IEnumerable<GetOrderHistoriesResponse>> GetOrderHistories(Guid orderId)
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<GetOrderHistoriesResponse>> GetOrderHistories(Guid orderId)
        //{
        //    IEnumerable<GetOrderHistoriesResponse> respone = await _unitOfWork.GetRepository<OrderHistory>().GetListAsync(
        //       selector: x => _mapper.Map<GetOrderHistoriesResponse>(x),
        //       include: x => x.Include(x => x.User),
        //       orderBy: x => x.OrderByDescending(x => x.CreatedDate));
        //    return respone;
        //}
    }
}

