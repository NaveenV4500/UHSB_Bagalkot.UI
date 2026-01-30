using AutoMapper;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels.CartOrder;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class OrderRepository : CommonConnection
    {
        private readonly IMapper _mapper;

        public OrderRepository(Uhsb2025uatContext context, IMapper mapper)
          : base(context)
        {
            _mapper = mapper;
        }
        public string GenerateOrderNumber(int lastOrderId)
        {
            return $"ORD-{DateTime.Now:yyyyMMdd}-{(lastOrderId + 1).ToString().PadLeft(6, '0')}";
        }

        public async Task<bool> SaveOrderAsync(OrderMasterVM orderVm)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLog");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            string filepath = Path.Combine(logPath, "OrderSaveLog.txt");


            try
            {
                WriteLog(filepath, "==================== Start Save Order ==========================");
                WriteLog(filepath, $"UserId : {orderVm.UserId}");

              
                int lastOrderId = _context.UhsbOrderMasters
                                          .OrderByDescending(x => x.OrderId)
                                          .Select(x => x.OrderId)
                                          .FirstOrDefault();

                orderVm.OrderNumber = GenerateOrderNumber(lastOrderId);
                WriteLog(filepath, $"Generated OrderNumber : {orderVm.OrderNumber}");
 
                var orderEntity = _mapper.Map<UhsbOrderMaster>(orderVm);
                 
                
                orderEntity.PaymentStatus = "No Payment";
                orderEntity.OrderDataStatusType = (byte)CommonEnum.OrderDataStatusType.Confirmed;
                orderEntity.CreatedDate = DateTime.Now;
                orderEntity.ModifiedDate = DateTime.Now;

                _context.UhsbOrderMasters.Add(orderEntity);
                var res = await _context.SaveChangesAsync();
                WriteLog(filepath, $"SaveChanges Result : {res}");

                if (res > 0)
                {
                    WriteLog(filepath, $"Order Saved Successfully. OrderId : {orderEntity.OrderId}");
                     
                    WriteLog(filepath, "==================== End Save Order (Success) ==========================");
                    return true;
                }

                WriteLog(filepath, "Order Save Failed. No rows affected.");
                WriteLog(filepath, "==================== End Save Order (Failed) ==========================");
                return false;
            }
            catch (Exception ex)
            {
                WriteLog(filepath, $"Exception : {ex.Message}");
                WriteLog(filepath, "==================== End Save Order (Exception) ==========================");
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatus(int orderId, CommonEnum.OrderDataStatusType selectedStatus,int userId)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLog");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            string filepath = Path.Combine(logPath, "OrderStatusUpdateLog.txt");

            try
            {
                WriteLog(filepath, $"==================== Start Admin Update OrderStatus ==========================");
                WriteLog(filepath, $"OrderId : {orderId}, SelectedStatus : {selectedStatus}");

                var order = await _context.UhsbOrderMasters.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    WriteLog(filepath, $"OrderId {orderId} not found.");
                    return false;
                }

                // 1️⃣ Update OrderDataStatusType from admin selection
                order.OrderDataStatusType = (byte)selectedStatus;
                 
                order.OrderStatus = selectedStatus switch
                {
                    CommonEnum.OrderDataStatusType.Pending => "Pending For Approval",
                    CommonEnum.OrderDataStatusType.Processing => "Processing",
                    CommonEnum.OrderDataStatusType.Confirmed => "Success",
                    CommonEnum.OrderDataStatusType.Delivered => "Delivered",
                    CommonEnum.OrderDataStatusType.Cancelled => "Cancelled",
                    _ => order.OrderStatus
                };

                order.ModifiedDate = DateTime.Now;
                order.ModifiedBy = userId;

                var res = await _context.SaveChangesAsync();

                WriteLog(filepath, $"OrderStatus updated successfully. SaveChanges Result: {res}");
                WriteLog(filepath, "==================== End Admin Update OrderStatus ==========================");
                return res > 0;
            }
            catch (Exception ex)
            {
                WriteLog(filepath, $"Exception : {ex.Message}");
                WriteLog(filepath, "==================== End Admin Update OrderStatus (Exception) ==========================");
                throw;
            }
        }



        public async Task AddOrderItems(List<OrderItemVM> items)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLog");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            string filepath = Path.Combine(logPath, "OrderItemLog.txt");

            try
            {
                WriteLog(filepath, "---------- Start AddOrderItems ----------");
                WriteLog(filepath, $"Items Count : {items.Count}");

                var orderItems = _mapper.Map<List<UhsbOrderItem>>(items);
                WriteLog(filepath, "Order items mapped successfully");

                _context.UhsbOrderItems.AddRange(orderItems);
                var res = await _context.SaveChangesAsync();

                WriteLog(filepath, $"Order items saved. Rows affected : {res}");
                WriteLog(filepath, "---------- End AddOrderItems (Success) ----------");
            }
            catch (Exception ex)
            {
                WriteLog(filepath, $"Exception in AddOrderItems : {ex.Message}");
                WriteLog(filepath, "---------- End AddOrderItems (Exception) ----------");
                throw;
            }
        }

        public async Task<OrderMasterVM> GetOrderById(int orderId)
        {
            var orderEntity = _context.UhsbOrderMasters.Include(x => x.UhsbOrderItems).FirstOrDefault(x => x.OrderId == orderId);

            if (orderEntity == null)
                return null;

            return _mapper.Map<OrderMasterVM>(orderEntity);
        }


        public async Task<List<OrderItemVM>> GetOrderItemsByOrderId(int orderId)
        {
            var orderitem = _context.UhsbOrderItems.Where(x => x.OrderId == orderId).ToListAsync();

            if (orderitem == null)
                return null;

            return _mapper.Map<List<OrderItemVM>>(orderitem);
        }

        public async Task<List<OrderMasterVM>> GetOrdersByUserId(int userId)
        {
            var orderEntity = _context.UhsbOrderMasters.Where(x => x.UserId == userId).OrderByDescending(x => x.OrderDate).ToList();
            if (orderEntity == null)
                return null;

            return _mapper.Map<List<OrderMasterVM>>(orderEntity);
        }


        public void WriteLog(string logPath, string message)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
            }
        }


    }
}
