﻿using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassRecordEvaluateDAL : IBaseDAL
    {
        Task<bool> AddClassRecordEvaluateStudent(EtClassRecordEvaluateStudent entity);

        Task<bool> AddClassRecordEvaluateTeacher(EtClassRecordEvaluateTeacher entity);

        Task<List<EtClassRecordEvaluateStudent>> GetClassRecordEvaluateStudent(long classRecordStudentId);

        Task<Tuple<IEnumerable<EtClassRecordEvaluateStudent>, int>> GetEvaluateStudentPaging(RequestPagingBase request);

        Task<Tuple<IEnumerable<EtClassRecordEvaluateTeacher>, int>> GetEvaluateTeacherPaging(RequestPagingBase request);
    }
}
