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
using ETMS.Entity.Dto.External.Request;

namespace ETMS.Business.Common
{
    public class ExcelLib
    {
        public static CheckImportStudentTemplateFileResult CheckImportStudentExcelTemplate(string tenantCode, string serverPath)
        {
            var fileName = $"潜在学员导入模板_{tenantCode}.xlsx";
            return FileHelper.CheckImportStudentTemplateFile(serverPath, fileName);
        }

        public static List<string> GetImportStudentHeadDesc()
        {
            var headList = new List<string>();
            headList.Add("学员姓名(*)");
            headList.Add("手机号码(*)");
            headList.Add("联系人角色");
            headList.Add("性别");
            headList.Add("出生日期");
            headList.Add("就读学校");
            headList.Add("就读年级");
            headList.Add("学员来源");
            headList.Add("备用号码");
            headList.Add("家庭住址");
            headList.Add("备注");
            return headList;
        }

        public static void GenerateImportStudentExcelTemplate(ImportStudentExcelTemplateRequest request)
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
            noteString.Append("4.模板中部分列内容提供下拉列表选择，请勿填写其它内容\n");
            notesTitle.SetCellValue(noteString.ToString());
            notesTitle.CellStyle = notesStyle;
            rowRemind.Height = 1800; ;
            var headTitleDesc = GetImportStudentHeadDesc();

            var rowHead = sheet1.CreateRow(1);
            var styleHead = workMbrTemplate.CreateCellStyle();
            var fontHead = workMbrTemplate.CreateFont();
            fontHead.Boldweight = (short)FontBoldWeight.Bold; ;
            styleHead.SetFont(fontHead);
            var formatHead = workMbrTemplate.CreateDataFormat();
            styleHead.DataFormat = formatHead.GetFormat("text");

            var cellHead0 = rowHead.CreateCell(0);
            cellHead0.CellStyle = styleHead;
            cellHead0.SetCellValue(headTitleDesc[0]);


            var cellHead1 = rowHead.CreateCell(1);
            cellHead1.CellStyle = styleHead;
            cellHead1.SetCellValue(headTitleDesc[1]);

            var dvHelper = new XSSFDataValidationHelper((XSSFSheet)sheet1);

            var cellHead2 = rowHead.CreateCell(2);
            cellHead2.CellStyle = styleHead;
            cellHead2.SetCellValue(headTitleDesc[2]);

            var regions2 = new CellRangeAddressList(1, 65535, 2, 2);
            var dataValidate2 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.StudentRelationshipAll.Select(p => p.Name).ToArray()), regions2);
            sheet1.AddValidationData(dataValidate2);

            var cellHead3 = rowHead.CreateCell(3);
            cellHead3.CellStyle = styleHead;
            cellHead3.SetCellValue(headTitleDesc[3]);

            var regions3 = new CellRangeAddressList(1, 65535, 3, 3);
            var dataValidate3 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(new string[] { "男", "女" }), regions3);
            sheet1.AddValidationData(dataValidate3);

            var cellHead4 = rowHead.CreateCell(4);
            cellHead4.CellStyle = styleHead;
            cellHead4.SetCellValue(headTitleDesc[4]);
            var cellStyle = workMbrTemplate.CreateCellStyle();
            var format = workMbrTemplate.CreateDataFormat();
            cellStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
            sheet1.SetDefaultColumnStyle(4, cellStyle);
            sheet1.SetColumnWidth(4, sheet1.GetColumnWidth(4) * 2);

            var cellHead5 = rowHead.CreateCell(5);
            cellHead5.CellStyle = styleHead;
            cellHead5.SetCellValue(headTitleDesc[5]);

            var cellHead6 = rowHead.CreateCell(6);
            cellHead6.CellStyle = styleHead;
            cellHead6.SetCellValue(headTitleDesc[6]);

            var regions6 = new CellRangeAddressList(1, 65535, 6, 6);
            var dataValidate6 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.GradeAll.Select(p => p.Name).ToArray()), regions6);
            sheet1.AddValidationData(dataValidate6);

            var cellHead7 = rowHead.CreateCell(7);
            cellHead7.CellStyle = styleHead;
            cellHead7.SetCellValue(headTitleDesc[7]);

            var regions7 = new CellRangeAddressList(1, 65535, 7, 7);
            var dataValidate7 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.StudentSourceAll.Select(p => p.Name).ToArray()), regions7);
            sheet1.AddValidationData(dataValidate7);

            var cellHead8 = rowHead.CreateCell(8);
            cellHead8.CellStyle = styleHead;
            cellHead8.SetCellValue(headTitleDesc[8]);

            var cellHead9 = rowHead.CreateCell(9);
            cellHead9.CellStyle = styleHead;
            cellHead9.SetCellValue(headTitleDesc[9]);

            var cellHead10 = rowHead.CreateCell(10);
            cellHead10.CellStyle = styleHead;
            cellHead10.SetCellValue(headTitleDesc[10]);

            using (var fs = File.OpenWrite(request.CheckResult.StrFileFullPath))
            {
                workMbrTemplate.Write(fs);
            }
        }

        public static string GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }
            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    short format = cell.CellStyle.DataFormat;
                    if (format != 0)
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                default:
                    return string.Empty;
            }
        }

        public static Tuple<string, List<ImportStudentContentItem>> ReadImportStudentExcelContent(Stream excelStream, int sheetIndex, int validDataRowIndex)
        {
            var workbook = WorkbookFactory.Create(excelStream);
            var workSheet = workbook.GetSheetAt(sheetIndex);
            var outStudentContent = new List<ImportStudentContentItem>();
            if (workSheet.LastRowNum <= 1)
            {
                return Tuple.Create("请按要求填写学员信息", outStudentContent);
            }
            if (workSheet.LastRowNum > 1005)
            {
                return Tuple.Create("一次性最多导入1000条数据", outStudentContent);
            }

            var headRow = workSheet.GetRow(validDataRowIndex - 1);
            var headTitleDesc = GetImportStudentHeadDesc();
            for (var i = 0; i < headTitleDesc.Count; i++)
            {
                if (GetCellValue(headRow.GetCell(i)) != headTitleDesc[i])
                {
                    return Tuple.Create("请选择正确的excel模板文件导入", outStudentContent);
                }
            }

            var strError = new StringBuilder();
            var readRowIndex = validDataRowIndex;
            while (readRowIndex <= workSheet.LastRowNum)
            {
                var myRow = workSheet.GetRow(readRowIndex);
                if (myRow == null)
                {
                    strError.Append($"第{readRowIndex + 1}行数据无效，请重新检验</br>");
                    continue;
                }
                var myStudentItem = new ImportStudentContentItem();
                var i = myRow.FirstCellNum;

                var studentNameValue = GetCellValue(myRow.GetCell(i));
                if (string.IsNullOrEmpty(studentNameValue))
                {
                    strError.Append($"第{readRowIndex + 1}行学员姓名不能为空</br>");
                }
                else
                {
                    myStudentItem.StudentName = studentNameValue;
                }

                var phoneCellValue = GetCellValue(myRow.GetCell(++i));
                if (string.IsNullOrEmpty(phoneCellValue) || !EtmsHelper.IsMobilePhone(phoneCellValue))
                {
                    strError.Append($"第{readRowIndex + 1}行手机号码不正确</br>");
                }
                else
                {
                    myStudentItem.Phone = phoneCellValue;
                }

                myStudentItem.PhoneRelationshipDesc = GetCellValue(myRow.GetCell(++i));

                myStudentItem.GenderDesc = GetCellValue(myRow.GetCell(++i));

                var birthdayCellValue = GetCellValue(myRow.GetCell(++i));
                if (!string.IsNullOrEmpty(birthdayCellValue))
                {
                    if (DateTime.TryParse(birthdayCellValue, out DateTime tempTime))
                    {
                        myStudentItem.Birthday = tempTime;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行出生日期格式不正确</br>");
                    }
                }

                myStudentItem.SchoolName = GetCellValue(myRow.GetCell(++i));

                myStudentItem.GradeDesc = GetCellValue(myRow.GetCell(++i));

                myStudentItem.SourceDesc = GetCellValue(myRow.GetCell(++i));

                myStudentItem.PhoneBak = GetCellValue(myRow.GetCell(++i));

                myStudentItem.HomeAddress = GetCellValue(myRow.GetCell(++i));

                myStudentItem.Remark = GetCellValue(myRow.GetCell(++i));

                outStudentContent.Add(myStudentItem);
                readRowIndex++;
            }
            return Tuple.Create(strError.ToString(), outStudentContent);
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
