﻿using eSMP.VModels;

namespace eSMP.Services.AfterBuyServiceRepo
{
    public interface IAfterBuyServiceReposity
    {
        public Result CreateChangeService(AfterBuyServiceModel serviceCreate);
        public Result AcceptService(int serviceID);
        public Result CancelService(int serviceID, string reason);
        public Result WarningService(int serviceID);
    }
}
