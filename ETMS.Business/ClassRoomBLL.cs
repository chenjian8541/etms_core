using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ClassRoomBLL : IClassRoomBLL
    {
        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public ClassRoomBLL(IClassRoomDAL classRoomDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._classRoomDAL = classRoomDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRoomDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> ClassRoomAdd(ClassRoomAddRequest request)
        {
            await _classRoomDAL.AddClassRoom(new EtClassRoom()
            {
                TenantId = request.LoginTenantId,
                Remark = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name
            });
            await _userOperationLogDAL.AddUserLog(request, $"教室设置:{request.Name}", EmUserOperationType.ClassRoomSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassRoomGet(ClassRoomGetRequest request)
        {
            var classRooms = await _classRoomDAL.GetAllClassRoom();
            return ResponseBase.Success(classRooms.Select(p => new ClassRoomViewOutput()
            {
                CId = p.Id,
                Name = p.Name
            }));
        }

        public async Task<ResponseBase> ClassRoomDel(ClassRoomDelRequest request)
        {
            await _classRoomDAL.DelClassRoom(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除教室设置", EmUserOperationType.ClassRoomSetting);
            return ResponseBase.Success();
        }
    }
}
