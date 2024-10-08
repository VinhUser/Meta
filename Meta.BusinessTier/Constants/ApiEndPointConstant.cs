﻿using System.Net.NetworkInformation;

namespace Meta.BusinessTier.Constants;

public static class ApiEndPointConstant
{

    public const string RootEndPoint = "/api";
    public const string ApiVersion = "/v1";
    public const string ApiEndpoint = RootEndPoint + ApiVersion;
    public static class Payment
    {
        public const string PaymentEndpoint = ApiEndpoint + "/payments";
        public const string VnPayEndpoint = PaymentEndpoint + "/vnpay";
        public const string PaymentsEndpoint = PaymentEndpoint + "/{id}";

    }
    public static class Authentication
    {
        public const string AuthenticationEndpoint = ApiEndpoint + "/auth";
        public const string Login = AuthenticationEndpoint + "/login";
    }
    public static class DashBoard
    {
        public const string DashBoardEndpoint = ApiEndpoint + "/dashboard";
        public const string DashBoardCountAccountEndpoint = DashBoardEndpoint + "/countAccount";
        public const string DashBoardCountOrderInYearEndpoint = DashBoardEndpoint + "/countOrderInYeay";
        public const string DashBoardCountOrderInMonthEndpoint = DashBoardEndpoint + "/countOrderInMonth";
        public const string DashBoardCountOrderEndpoint = DashBoardEndpoint + "/countOrder";
    }

    public static class Rank
    {
        public const string RanksEndPoint = ApiEndpoint + "/rank";
        public const string RanksEndPointRankToAccount = RanksEndPoint + "/{id}/account";
        public const string RankEndPoint = RanksEndPoint + "/{id}";
        public const string RanksUpdateStatusEndPoint = RanksEndPoint + "/updateStatus/{id}";

    }
    public static class Account
    {
        public const string AccountsEndPoint = ApiEndpoint + "/accounts";
        public const string StaffEndPoint = ApiEndpoint + "/staff";
        public const string AccountEndPoint = AccountsEndPoint + "/{id}";
        public const string AccountEndPointMember = AccountsEndPoint + "/{id}";
        public const string AccountToRankEndPoint = AccountsEndPoint + "/{id}/addRank";
        public const string AccountEndPointChangePassword = AccountsEndPoint + "/{id}/password";
        public const string StaffEndPoints = ApiEndpoint + "/register/staff";
        public const string CustomerEndPoint = ApiEndpoint + "/register/customer";
        public const string GetTaskOfStaff = StaffEndPoint + "/task";

    }

    public static class Category
    {
        public const string CategoriesEndPoint = ApiEndpoint + "/categories";
        public const string CategoryEndPoint = CategoriesEndPoint + "/{id}";
        public const string CategoryAddDiscountEndPoint = CategoryEndPoint + "/discount";
        public const string CategoriesNopagingNateEndPoint = CategoriesEndPoint + "/noPagingNate";
    }
    public static class CustomerRequest
    {
        public const string CustomerRequestsEndPoint = ApiEndpoint + "/request";
        public const string CustomerRequestEndPoint = CustomerRequestsEndPoint + "/{id}";
        public const string CustomerRequestsNopagingNateEndPoint = CustomerRequestsEndPoint + "/noPagingNate";
    }
    public static class TypePet
    {
        public const string TypePetsEndPoint = ApiEndpoint + "/typePet";
        public const string TypePetEndPoint = TypePetsEndPoint + "/{id}";
        public const string TypePetAddDiscountEndPoint = TypePetEndPoint + "/discount";
        public const string TypePetsNopagingNateEndPoint = TypePetEndPoint + "/noPagingNate";
    }
    public static class Product
    {
        public const string ProductsEndPoint = ApiEndpoint + "/products";
        public const string ProductsEndPointNoPaginate = ApiEndpoint + "/products/noPaginat";
        public const string ProductEndPoint = ProductsEndPoint + "/{id}";
        public const string AddSupProductToProductEndPoint = ProductEndPoint + "/supProduct";

    }
    public static class Pet
    {
        public const string PetsEndPoint = ApiEndpoint + "/pet";
        public const string PetsEndPointNoPaginate = ApiEndpoint + "/pet/noPaginat";
        public const string PetEndPoint = PetsEndPoint + "/{id}";

    }
    public static class SupProduct
    {
        public const string SupProductsEndPoint = ApiEndpoint + "/supProducts";
        public const string SupProductsEndPointNoPaginate = ApiEndpoint + "/supProducts/noPaginat";
        public const string SupProductEndPoint = SupProductsEndPoint + "/{id}";

    }


    public static class Order
    {
        public const string OrdersEndPoint = ApiEndpoint + "/orders";
        public const string OrderEndPoint = OrdersEndPoint + "/{id}";
        public const string OrderHistoriesEndPoint = OrderEndPoint + "/orderHistory";
    }
    public static class TaskManager
    {
        public const string TasksEndPoint = ApiEndpoint + "/task";
        public const string TaskEndPoint = TasksEndPoint + "/{id}";

    }
}