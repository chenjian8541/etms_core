using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData2.Output;
using ETMS.Entity.Dto.HisData2.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class MallOrderBLL : IMallOrderBLL
    {
        private readonly IMallOrderDAL _mallOrderDAL;

        private readonly IStudentDAL _studentDAL;

        public MallOrderBLL(IMallOrderDAL mallOrderDAL, IStudentDAL studentDAL)
        {
            this._mallOrderDAL = mallOrderDAL;
            this._studentDAL = studentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _mallOrderDAL, _studentDAL);
        }

        public async Task<ResponseBase> MallOrderGetPaging(MallOrderGetPagingRequest request)
        {
            var pagingData = await _mallOrderDAL.GetPaging(request);
            var output = new List<MallOrderGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                foreach (var p in pagingData.Item1)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    if (myStudent == null)
                    {
                        continue;
                    }
                    List<ParentBuyMallGoodsSubmitSpecItem> goodsSpecContent = null;
                    if (p.ProductType == EmProductType.Goods)
                    {
                        if (!string.IsNullOrEmpty(p.GoodsSpecContent))
                        {
                            goodsSpecContent = JsonConvert.DeserializeObject<List<ParentBuyMallGoodsSubmitSpecItem>>(p.GoodsSpecContent);
                        }
                    }
                    output.Add(new MallOrderGetPagingOutput()
                    {
                        StudentId = p.StudentId,
                        AptSum = p.AptSum,
                        BuyCount = p.BuyCount,
                        CId = p.Id,
                        CreateOt = p.CreateOt,
                        CreateTime = p.CreateTime,
                        GoodsName = p.GoodsName,
                        GoodsSpecContent = goodsSpecContent,
                        ProductType = p.ProductType,
                        ImgCoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.ImgCover),
                        LcsPayLogId = p.LcsPayLogId,
                        MallGoodsId = p.MallGoodsId,
                        OrderNo = p.OrderNo,
                        PaySum = p.PaySum,
                        PriceRuleDesc = p.PriceRuleDesc,
                        ProductTypeDesc = p.ProductTypeDesc,
                        RelatedId = p.RelatedId,
                        Remark = p.Remark,
                        Status = p.Status,
                        StatusDesc = EmMallOrderStatus.GetMallOrderStatusDesc(p.Status),
                        StudentName = myStudent.Name,
                        StudentPhone = myStudent.Phone,
                        TotalPoints = p.TotalPoints,
                        OrderId = p.OrderId
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<MallOrderGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> MallOrderGet(MallOrderGetRequest request)
        {
            var p = await _mallOrderDAL.GetMallOrder(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("未找到此订单");
            }
            var myStudentBucket = await _studentDAL.GetStudent(p.StudentId);
            if (myStudentBucket == null || myStudentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var myStudent = myStudentBucket.Student;
            List<ParentBuyMallGoodsSubmitSpecItem> goodsSpecContent = null;
            if (p.ProductType == EmProductType.Course)
            {
                if (!string.IsNullOrEmpty(p.GoodsSpecContent))
                {
                    goodsSpecContent = JsonConvert.DeserializeObject<List<ParentBuyMallGoodsSubmitSpecItem>>(p.GoodsSpecContent);
                }
            }
            var output = new MallOrderGetPagingOutput()
            {
                StudentId = p.StudentId,
                AptSum = p.AptSum,
                BuyCount = p.BuyCount,
                CId = p.Id,
                CreateOt = p.CreateOt,
                CreateTime = p.CreateTime,
                GoodsName = p.GoodsName,
                GoodsSpecContent = goodsSpecContent,
                ProductType = p.ProductType,
                ImgCoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.ImgCover),
                LcsPayLogId = p.LcsPayLogId,
                MallGoodsId = p.MallGoodsId,
                OrderNo = p.OrderNo,
                PaySum = p.PaySum,
                PriceRuleDesc = p.PriceRuleDesc,
                ProductTypeDesc = p.ProductTypeDesc,
                RelatedId = p.RelatedId,
                Remark = p.Remark,
                Status = p.Status,
                StatusDesc = EmMallOrderStatus.GetMallOrderStatusDesc(p.Status),
                StudentName = myStudent.Name,
                StudentPhone = myStudent.Phone,
                TotalPoints = p.TotalPoints,
                OrderId = p.OrderId
            };
            return ResponseBase.Success(output);
        }
    }
}
