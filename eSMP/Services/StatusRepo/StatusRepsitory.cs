using eSMP.Models;

namespace eSMP.Services.StatusRepo
{
    public class StatusRepsitory : IStatusReposity
    {
        public Status GetfeedbackStatus(int statusID)
        {
            Status status = new Status();
            switch (statusID)
            {
                case 1:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Hoạt động"
                    };
                    break;
                case 2:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Khoá"
                    };
                    break;
                case 3:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Ẩn"
                    };
                    break;
                    break;
                default:
                    status = null;
                    break;
            }
            return status;
        }

        public Status GetItemStatus(int statusID)
        {
            Status status = new Status();
            switch (statusID)
            {
                case 1:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Hoạt động"
                    };
                    break;
                case 2:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Khoá"
                    };
                    break;
                case 3:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Chờ"
                    };
                    break;
                case 4:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Ẩn"
                    };
                    break;
                default:
                    status = null;
                    break;
            }
            return status;
        }

        public Status GetOrderStatus(int statusID)
        {
            Status status = new Status();
            switch (statusID)
            {
                case 1:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Đã thanh toán"
                    };
                    break;
                case 2:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "giỏ hàng"
                    };
                    break;
                case 3:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Huỷ"
                    };
                    break;
                default:
                    status = null;
                    break;
            }
            return status;
        }

        public Status GetReportStatus(int statusID)
        {
            Status status = new Status();
            switch (statusID)
            {
                case 1:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Hoạt động"
                    };
                    break;
                case 2:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "khoá"
                    };
                    break;
                case 3:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "chờ"
                    };
                    break;
                default:
                    status = null;
                    break;
            }
            return status;
        }

        public Status GetStoreStatus(int statusID)
        {
            Status status = new Status();
            switch (statusID)
            {
                case 1:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Hoạt động"
                    };
                    break;
                case 2:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Khoá"
                    };
                    break;
                case 3:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Chờ kích hoạt"
                    };
                    break;
                case 4:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Ẩn"
                    };
                    break;
                default:
                    status = null;
                    break;
            }
            return status;
        }

        public Status GetSubItemStatus(int statusID)
        {
            Status status = new Status();
            switch (statusID)
            {
                case 1:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Hoạt động"
                    };
                    break;
                case 2:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Khoá"
                    };
                    break;
                case 3:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Chờ"
                    };
                    break;
                case 4:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Ẩn"
                    };
                    break;
                default:
                    status = null;
                    break;
            }
            return status;
        }

        public Status GetWithdrawalStatus(int statusID)
        {
            Status status = new Status();
            switch (statusID)
            {
                case 1:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Chờ tiếp nhận"
                    };
                    break;
                case 2:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Đang xử lí"
                    };
                    break;
                case 3:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Huỷ"
                    };
                    break;
                case 4:
                    status = new Status
                    {
                        Item_StatusID = statusID,
                        StatusName = "Hoàn thành"
                    };
                    break;
                default:
                    status = null;
                    break;
            }
            return status;
        }
    }
}
