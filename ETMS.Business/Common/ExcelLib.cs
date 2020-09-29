using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using NPOI.SS.UserModel;

namespace ETMS.Business.Common
{
    internal class ExcelLib
    {
        internal static CheckImportStudentTemplateFileResult CheckImportStudentExcelTemplate(string tenantCode, string serverPath)
        {
            var fileName = $"潜在学员导入模板_{tenantCode}.xlsx";
            return FileHelper.CheckImportStudentTemplateFile(serverPath, fileName);
        }

        internal static void GenerateImportStudentExcelTemplate(ImportStudentExcelTemplateRequest request)
        {
            var workMbrTemplate = new XSSFWorkbook();
            var sheet1 = workMbrTemplate.CreateSheet("导入学员信息");
            sheet1.DefaultColumnWidth = sheet1.DefaultColumnWidth * 2;

            sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 12));
            var rowRemind = sheet1.CreateRow(0);
            var notesTitle = rowRemind.CreateCell(0);
            var notesStyle = workMbrTemplate.CreateCellStyle();
            notesStyle.WrapText = true;
            var noteString = new StringBuilder();
            noteString.Append("注意事项： \n");
            noteString.Append("1.请严格按照模版格式录入内容\n");
            noteString.Append("2.此表不允许做如增加列、删除列、修改表头名称等操作\n");
            noteString.Append("3.模板中标识为*号的栏目为必填项，导入时不能为空\n");
            noteString.Append("4.部分栏目内容提供下拉列表选择，请勿填写其它内容\n");
            notesTitle.SetCellValue(noteString.ToString());
            notesTitle.CellStyle = notesStyle;
            rowRemind.Height = 1800; ;

            var rowHead = sheet1.CreateRow(1);

            rowHead.CreateCell(0).SetCellValue("学员姓名(*)");

            rowHead.CreateCell(1).SetCellValue("手机号码(*)");

            var dvHelper = new XSSFDataValidationHelper((XSSFSheet)sheet1);

            rowHead.CreateCell(2).SetCellValue("联系人角色");
            var regions2 = new CellRangeAddressList(1, 65535, 2, 2);
            var dataValidate2 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.StudentRelationshipAll.Select(p => p.Name).ToArray()), regions2);
            sheet1.AddValidationData(dataValidate2);

            rowHead.CreateCell(3).SetCellValue("性别");
            var regions3 = new CellRangeAddressList(1, 65535, 3, 3);
            var dataValidate3 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(new string[] { "男", "女" }), regions3);
            sheet1.AddValidationData(dataValidate3);

            rowHead.CreateCell(4).SetCellValue("出生日期");
            var cellStyle = workMbrTemplate.CreateCellStyle();
            var format = workMbrTemplate.CreateDataFormat();
            cellStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
            sheet1.SetDefaultColumnStyle(4, cellStyle);
            sheet1.SetColumnWidth(4, sheet1.GetColumnWidth(4) * 2);

            rowHead.CreateCell(5).SetCellValue("就读学校");

            rowHead.CreateCell(6).SetCellValue("就读年级");
            var regions6 = new CellRangeAddressList(1, 65535, 6, 6);
            var dataValidate6 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.GradeAll.Select(p => p.Name).ToArray()), regions6);
            sheet1.AddValidationData(dataValidate6);

            rowHead.CreateCell(7).SetCellValue("学员来源");
            var regions7 = new CellRangeAddressList(1, 65535, 7, 7);
            var dataValidate7 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.StudentSourceAll.Select(p => p.Name).ToArray()), regions7);
            sheet1.AddValidationData(dataValidate7);

            rowHead.CreateCell(8).SetCellValue("备用号码");

            rowHead.CreateCell(9).SetCellValue("家庭住址");

            rowHead.CreateCell(10).SetCellValue("备注");

            using (var fs = File.OpenWrite(request.CheckResult.StrFileFullPath))
            {
                workMbrTemplate.Write(fs);
            }
        }
    }

    public class ImportStudentExcelTemplateRequest
    {
        public CheckImportStudentTemplateFileResult CheckResult { get; set; }

        public List<EtStudentRelationship> StudentRelationshipAll { get; set; }

        public List<EtGrade> GradeAll { get; set; }

        public List<EtStudentSource> StudentSourceAll { get; set; }

    }
}
