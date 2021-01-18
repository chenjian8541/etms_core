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
using ETMS.Entity.Enum;

namespace ETMS.Business.Common
{
    public class ExcelLib
    {
        #region 导入潜在学员

        public static CheckImportStudentTemplateFileResult CheckImportStudentExcelTemplate(string tenantCode, string serverPath)
        {
            var fileName = $"潜在学员导入模板_{tenantCode}.xlsx";
            return FileHelper.CheckImportExcelTemplateFile(serverPath, fileName);
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
            noteString.Append("5.文档中日期信息请按yyyy-MM-dd格式填写\n");
            notesTitle.SetCellValue(noteString.ToString());
            notesTitle.CellStyle = notesStyle;
            rowRemind.Height = 2200; ;
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
            if (request.StudentRelationshipAll.Count > 0)
            {
                var dataValidate2 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.StudentRelationshipAll.Select(p => p.Name).ToArray()), regions2);
                sheet1.AddValidationData(dataValidate2);
            }

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
            if (request.GradeAll.Count > 0)
            {
                var dataValidate6 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.GradeAll.Select(p => p.Name).ToArray()), regions6);
                sheet1.AddValidationData(dataValidate6);
            }

            var cellHead7 = rowHead.CreateCell(7);
            cellHead7.CellStyle = styleHead;
            cellHead7.SetCellValue(headTitleDesc[7]);

            var regions7 = new CellRangeAddressList(1, 65535, 7, 7);
            if (request.StudentSourceAll.Count > 0)
            {
                var dataValidate7 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.StudentSourceAll.Select(p => p.Name).ToArray()), regions7);
                sheet1.AddValidationData(dataValidate7);
            }

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
                if (strError.Length > 200)
                {
                    break;
                }
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


        #endregion


        #region 导入学员课时(按课时收费)

        public static CheckImportStudentTemplateFileResult CheckImportCourseTimesExcelTemplate(string tenantCode, string serverPath)
        {
            var fileName = $"学员课程导入模板(按课时)_{tenantCode}.xlsx";
            return FileHelper.CheckImportExcelTemplateFile(serverPath, fileName);
        }

        /// <summary>
        /// 按课时导入
        /// </summary>
        /// <returns></returns>
        public static List<string> GetImportCourseHeadDescTimes()
        {
            var headList = new List<string>();
            headList.Add("学员姓名(*)");
            headList.Add("手机号码(*)");
            headList.Add("课程名称(*)");
            headList.Add("购买课时数量(*)");
            headList.Add("赠送课时数量");
            headList.Add("剩余课时数量(*)");
            headList.Add("课程有效期");
            headList.Add("应收金额(*)");
            headList.Add("实收金额(*)");
            headList.Add("支付方式");
            headList.Add("经办日期");
            headList.Add("课程类型");
            return headList;
        }

        public static void GenerateImportCourseTimesExcelTemplate(ImportCourseHeadDescTimesExcelTemplateRequest request)
        {
            var workMbrTemplate = new XSSFWorkbook();
            var sheet1 = workMbrTemplate.CreateSheet("导入学员课程信息(按课时)");
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
            noteString.Append("5.课程类型默认是“一对多”，如果选择“一对一”则会自动创建该学员对应的一对一班级\n");
            noteString.Append("6.文档中日期信息请按yyyy-MM-dd格式填写\n");
            notesTitle.SetCellValue(noteString.ToString());
            notesTitle.CellStyle = notesStyle;
            rowRemind.Height = 2500; ;
            var headTitleDesc = GetImportCourseHeadDescTimes();

            var rowHead = sheet1.CreateRow(1);
            var styleHead = workMbrTemplate.CreateCellStyle();
            var fontHead = workMbrTemplate.CreateFont();
            fontHead.Boldweight = (short)FontBoldWeight.Bold; ;
            styleHead.SetFont(fontHead);
            var formatHead = workMbrTemplate.CreateDataFormat();
            styleHead.DataFormat = formatHead.GetFormat("text");

            var cellHead0 = rowHead.CreateCell(0);  //学员姓名
            cellHead0.CellStyle = styleHead;
            cellHead0.SetCellValue(headTitleDesc[0]);


            var cellHead1 = rowHead.CreateCell(1);  //手机号码
            cellHead1.CellStyle = styleHead;
            cellHead1.SetCellValue(headTitleDesc[1]);

            var dvHelper = new XSSFDataValidationHelper((XSSFSheet)sheet1);

            var cellHead2 = rowHead.CreateCell(2);  //课程名称
            cellHead2.CellStyle = styleHead;
            cellHead2.SetCellValue(headTitleDesc[2]);

            var cellHead3 = rowHead.CreateCell(3);  //购买课时数量
            cellHead3.CellStyle = styleHead;
            cellHead3.SetCellValue(headTitleDesc[3]);
            var regions3 = new CellRangeAddressList(1, 65535, 3, 3);
            var dataValidate3 = dvHelper.CreateValidation(dvHelper.CreateNumericConstraint(ValidationType.INTEGER, OperatorType.BETWEEN, "1", "1000000"), regions3);
            dataValidate3.CreateErrorBox("错误", "购买课时数量必须为整数");
            sheet1.AddValidationData(dataValidate3);

            var cellHead4 = rowHead.CreateCell(4);  //赠送课时数量
            cellHead4.CellStyle = styleHead;
            cellHead4.SetCellValue(headTitleDesc[4]);
            var regions4 = new CellRangeAddressList(1, 65535, 4, 4);
            var dataValidate4 = dvHelper.CreateValidation(dvHelper.CreateNumericConstraint(ValidationType.INTEGER, OperatorType.BETWEEN, "1", "1000000"), regions4);
            dataValidate4.CreateErrorBox("错误", "赠送课时数量必须为整数");
            sheet1.AddValidationData(dataValidate4);

            var cellHead5 = rowHead.CreateCell(5);  //剩余课时数量
            cellHead5.CellStyle = styleHead;
            cellHead5.SetCellValue(headTitleDesc[5]);
            var regions5 = new CellRangeAddressList(1, 65535, 5, 5);
            var dataValidate5 = dvHelper.CreateValidation(dvHelper.CreateNumericConstraint(ValidationType.DECIMAL, OperatorType.BETWEEN, "1", "1000000"), regions5);
            dataValidate5.CreateErrorBox("错误", "剩余课时数量必须为数值类型");
            sheet1.AddValidationData(dataValidate5);


            var cellHead6 = rowHead.CreateCell(6);  //课程有效期
            cellHead6.CellStyle = styleHead;
            cellHead6.SetCellValue(headTitleDesc[6]);
            var cellStyle = workMbrTemplate.CreateCellStyle();
            var format = workMbrTemplate.CreateDataFormat();
            cellStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
            sheet1.SetDefaultColumnStyle(6, cellStyle);

            var cellHead7 = rowHead.CreateCell(7);  //应收金额
            cellHead7.CellStyle = styleHead;
            cellHead7.SetCellValue(headTitleDesc[7]);
            var regions7 = new CellRangeAddressList(1, 65535, 7, 7);
            var dataValidate7 = dvHelper.CreateValidation(dvHelper.CreateNumericConstraint(ValidationType.DECIMAL, OperatorType.BETWEEN, "1", "10000000"), regions7);
            dataValidate7.CreatePromptBox("错误", "应收金额必须为数值类型");
            sheet1.AddValidationData(dataValidate7);

            var cellHead8 = rowHead.CreateCell(8);  //实收金额
            cellHead8.CellStyle = styleHead;
            cellHead8.SetCellValue(headTitleDesc[8]);
            var regions8 = new CellRangeAddressList(1, 65535, 8, 8);
            var dataValidate8 = dvHelper.CreateValidation(dvHelper.CreateNumericConstraint(ValidationType.DECIMAL, OperatorType.BETWEEN, "1", "10000000"), regions8);
            dataValidate8.CreatePromptBox("错误", "应收金额必须为数值类型");
            sheet1.AddValidationData(dataValidate8);


            var cellHead9 = rowHead.CreateCell(9);  //支付方式
            cellHead9.CellStyle = styleHead;
            cellHead9.SetCellValue(headTitleDesc[9]);
            var regions9 = new CellRangeAddressList(1, 65535, 9, 9);
            var dataValidate9 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.PayTypeAll), regions9);
            sheet1.AddValidationData(dataValidate9);

            var cellHead10 = rowHead.CreateCell(10);  //经办日期
            cellHead10.CellStyle = styleHead;
            cellHead10.SetCellValue(headTitleDesc[10]);
            var cellStyle10 = workMbrTemplate.CreateCellStyle();
            var format10 = workMbrTemplate.CreateDataFormat();
            cellStyle10.DataFormat = format10.GetFormat("yyyy-MM-dd");
            sheet1.SetDefaultColumnStyle(10, cellStyle10);

            var cellHead11 = rowHead.CreateCell(11);  //课程类型
            cellHead11.CellStyle = styleHead;
            cellHead11.SetCellValue(headTitleDesc[11]);
            var regions11 = new CellRangeAddressList(1, 65535, 11, 11);
            var dataValidate11 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(new string[] { "一对多", "一对一" }), regions11);
            sheet1.AddValidationData(dataValidate11);

            using (var fs = File.OpenWrite(request.CheckResult.StrFileFullPath))
            {
                workMbrTemplate.Write(fs);
            }
        }

        public static Tuple<string, List<ImportCourseTimesItem>> ReadImportCourseTimesExcelContent(Stream excelStream, int sheetIndex, int validDataRowIndex)
        {
            var workbook = WorkbookFactory.Create(excelStream);
            var workSheet = workbook.GetSheetAt(sheetIndex);
            var outStudentContent = new List<ImportCourseTimesItem>();
            if (workSheet.LastRowNum <= 1)
            {
                return Tuple.Create("请按要求填写学员信息", outStudentContent);
            }
            if (workSheet.LastRowNum > 1005)
            {
                return Tuple.Create("一次性最多导入1000条数据", outStudentContent);
            }

            var headRow = workSheet.GetRow(validDataRowIndex - 1);
            var headTitleDesc = GetImportCourseHeadDescTimes();
            for (var i = 0; i < headTitleDesc.Count; i++)
            {
                if (GetCellValue(headRow.GetCell(i)) != headTitleDesc[i])
                {
                    return Tuple.Create("请选择正确的excel模板文件导入", outStudentContent);
                }
            }

            var strError = new StringBuilder();
            var readRowIndex = validDataRowIndex;
            var now = DateTime.Now;
            while (readRowIndex <= workSheet.LastRowNum)
            {
                if (strError.Length > 200)
                {
                    break;
                }
                var myRow = workSheet.GetRow(readRowIndex);
                if (myRow == null)
                {
                    strError.Append($"第{readRowIndex + 1}行数据无效，请重新检验</br>");
                    continue;
                }
                var myStudentCourseItem = new ImportCourseTimesItem();
                var i = myRow.FirstCellNum;

                var studentNameValue = GetCellValue(myRow.GetCell(i));    //学员姓名
                if (string.IsNullOrEmpty(studentNameValue))
                {
                    strError.Append($"第{readRowIndex + 1}行学员姓名不能为空</br>");
                }
                else
                {
                    myStudentCourseItem.StudentName = studentNameValue;
                }

                var phoneCellValue = GetCellValue(myRow.GetCell(++i));   //手机号码
                if (string.IsNullOrEmpty(phoneCellValue) || !EtmsHelper.IsMobilePhone(phoneCellValue))
                {
                    strError.Append($"第{readRowIndex + 1}行手机号码不正确</br>");
                }
                else
                {
                    myStudentCourseItem.Phone = phoneCellValue;
                }

                var courseNameCellValue = GetCellValue(myRow.GetCell(++i));   //课程名称
                if (string.IsNullOrEmpty(courseNameCellValue))
                {
                    strError.Append($"第{readRowIndex + 1}行课程名称不能为空</br>");
                }
                else
                {
                    myStudentCourseItem.CourseName = courseNameCellValue;
                }

                var buyQuantityCellValue = GetCellValue(myRow.GetCell(++i));  //购买课时数量
                if (string.IsNullOrEmpty(buyQuantityCellValue))
                {
                    strError.Append($"第{readRowIndex + 1}行购买课时数量不能为空</br>");
                }
                else
                {
                    if (int.TryParse(buyQuantityCellValue, out var mytempBuyQuantity))
                    {
                        if (mytempBuyQuantity <= 0)
                        {
                            strError.Append($"第{readRowIndex + 1}行购买课时数量必须大于0</br>");
                        }
                        myStudentCourseItem.BuyQuantity = mytempBuyQuantity;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行购买课时数量必须为整数</br>");
                    }
                }

                var giveQuantity = GetCellValue(myRow.GetCell(++i));  //赠送课时数量
                if (string.IsNullOrEmpty(giveQuantity))
                {
                    myStudentCourseItem.GiveQuantity = 0;
                }
                else
                {
                    if (int.TryParse(giveQuantity, out var mytempgiveQuantity))
                    {
                        myStudentCourseItem.GiveQuantity = mytempgiveQuantity;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行赠送课时数量必须为整数</br>");
                    }
                }

                var surplusQuantity = GetCellValue(myRow.GetCell(++i));  //剩余课时数量
                if (string.IsNullOrEmpty(surplusQuantity))
                {
                    strError.Append($"第{readRowIndex + 1}行剩余课时数量不能为空</br>");
                }
                else
                {
                    if (decimal.TryParse(surplusQuantity, out var mytempsurplusQuantity))
                    {
                        myStudentCourseItem.SurplusQuantity = mytempsurplusQuantity;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行剩余课时数量必须为数值类型</br>");
                    }
                }

                var endTime = GetCellValue(myRow.GetCell(++i));  //课程有效期
                if (string.IsNullOrEmpty(endTime))
                {
                    myStudentCourseItem.EndTime = null;
                }
                else
                {
                    if (DateTime.TryParse(endTime, out DateTime mytempendTime))
                    {
                        myStudentCourseItem.EndTime = mytempendTime;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行课程有效期格式不正确</br>");
                    }
                }

                var aptSum = GetCellValue(myRow.GetCell(++i));  //应收金额
                if (string.IsNullOrEmpty(aptSum))
                {
                    strError.Append($"第{readRowIndex + 1}行应收金额不能为空</br>");
                }
                else
                {
                    if (decimal.TryParse(aptSum, out var mytempaptSum))
                    {
                        myStudentCourseItem.AptSum = mytempaptSum;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行应收金额必须为数值类型</br>");
                    }
                }

                var paySum = GetCellValue(myRow.GetCell(++i));  //实收金额
                if (string.IsNullOrEmpty(paySum))
                {
                    strError.Append($"第{readRowIndex + 1}行实收金额不能为空</br>");
                }
                else
                {
                    if (decimal.TryParse(paySum, out var mytemppaySum))
                    {
                        myStudentCourseItem.PaySum = mytemppaySum;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行实收金额必须为数值类型</br>");
                    }
                }

                myStudentCourseItem.PayTypeName = GetCellValue(myRow.GetCell(++i));   //支付方式

                var orderOt = GetCellValue(myRow.GetCell(++i));   //经办日期
                if (string.IsNullOrEmpty(orderOt))
                {
                    myStudentCourseItem.OrderOt = now;
                }
                else
                {
                    if (DateTime.TryParse(orderOt, out DateTime mytemporderOt))
                    {
                        myStudentCourseItem.OrderOt = mytemporderOt;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行经办日期格式不正确</br>");
                    }
                }

                var courseType = GetCellValue(myRow.GetCell(++i));   //经办日期

                if (courseType == "一对一")
                {
                    myStudentCourseItem.CourseType = EmCourseType.OneToOne;
                }

                outStudentContent.Add(myStudentCourseItem);
                readRowIndex++;
            }
            return Tuple.Create(strError.ToString(), outStudentContent);
        }

        #endregion

        #region 导入学员课时(按月收费课程)

        public static CheckImportStudentTemplateFileResult CheckImportCourseDayExcelTemplate(string tenantCode, string serverPath)
        {
            var fileName = $"学员课程导入模板(按时间)_{tenantCode}.xlsx";
            return FileHelper.CheckImportExcelTemplateFile(serverPath, fileName);
        }

        /// <summary>
        /// 按课时导入
        /// </summary>
        /// <returns></returns>
        public static List<string> GetImportCourseHeadDescDay()
        {
            var headList = new List<string>();
            headList.Add("学员姓名(*)");
            headList.Add("手机号码(*)");
            headList.Add("课程名称(*)");
            headList.Add("课程开始日期(*)");
            headList.Add("课程结束日期(*)");
            headList.Add("应收金额(*)");
            headList.Add("实收金额(*)");
            headList.Add("支付方式");
            headList.Add("经办日期");
            headList.Add("课程类型");
            return headList;
        }

        public static void GenerateImportCourseDayExcelTemplate(ImportCourseHeadDescDayExcelTemplateRequest request)
        {
            var workMbrTemplate = new XSSFWorkbook();
            var sheet1 = workMbrTemplate.CreateSheet("导入学员课程信息(按时间)");
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
            noteString.Append("5.课程类型默认是“一对多”，如果选择“一对一”则会自动创建该学员对应的一对一班级\n");
            noteString.Append("6.文档中日期信息请按yyyy-MM-dd格式填写\n");
            notesTitle.SetCellValue(noteString.ToString());
            notesTitle.CellStyle = notesStyle;
            rowRemind.Height = 2500; ;
            var headTitleDesc = GetImportCourseHeadDescDay();

            var rowHead = sheet1.CreateRow(1);
            var styleHead = workMbrTemplate.CreateCellStyle();
            var fontHead = workMbrTemplate.CreateFont();
            fontHead.Boldweight = (short)FontBoldWeight.Bold; ;
            styleHead.SetFont(fontHead);
            var formatHead = workMbrTemplate.CreateDataFormat();
            styleHead.DataFormat = formatHead.GetFormat("text");

            var cellHead0 = rowHead.CreateCell(0);  //学员姓名
            cellHead0.CellStyle = styleHead;
            cellHead0.SetCellValue(headTitleDesc[0]);


            var cellHead1 = rowHead.CreateCell(1);  //手机号码
            cellHead1.CellStyle = styleHead;
            cellHead1.SetCellValue(headTitleDesc[1]);

            var dvHelper = new XSSFDataValidationHelper((XSSFSheet)sheet1);

            var cellHead2 = rowHead.CreateCell(2);  //课程名称
            cellHead2.CellStyle = styleHead;
            cellHead2.SetCellValue(headTitleDesc[2]);

            var cellHead3 = rowHead.CreateCell(3);  //课程开始日期
            cellHead3.CellStyle = styleHead;
            cellHead3.SetCellValue(headTitleDesc[3]);
            var cellStyle3 = workMbrTemplate.CreateCellStyle();
            var format3 = workMbrTemplate.CreateDataFormat();
            cellStyle3.DataFormat = format3.GetFormat("yyyy-MM-dd");
            sheet1.SetDefaultColumnStyle(3, cellStyle3);

            var cellHead4 = rowHead.CreateCell(4);  //课程结束日期
            cellHead4.CellStyle = styleHead;
            cellHead4.SetCellValue(headTitleDesc[4]);
            var cellStyle4 = workMbrTemplate.CreateCellStyle();
            var format4 = workMbrTemplate.CreateDataFormat();
            cellStyle4.DataFormat = format4.GetFormat("yyyy-MM-dd");
            sheet1.SetDefaultColumnStyle(4, cellStyle4);

            var cellHead5 = rowHead.CreateCell(5);  //应收金额
            cellHead5.CellStyle = styleHead;
            cellHead5.SetCellValue(headTitleDesc[5]);
            var regions5 = new CellRangeAddressList(1, 65535, 5, 5);
            var dataValidate5 = dvHelper.CreateValidation(dvHelper.CreateNumericConstraint(ValidationType.DECIMAL, OperatorType.BETWEEN, "1", "10000000"), regions5);
            dataValidate5.CreatePromptBox("错误", "应收金额必须为数值类型");
            sheet1.AddValidationData(dataValidate5);

            var cellHead6 = rowHead.CreateCell(6);  //实收金额
            cellHead6.CellStyle = styleHead;
            cellHead6.SetCellValue(headTitleDesc[6]);
            var regions6 = new CellRangeAddressList(1, 65535, 6, 6);
            var dataValidate6 = dvHelper.CreateValidation(dvHelper.CreateNumericConstraint(ValidationType.DECIMAL, OperatorType.BETWEEN, "1", "10000000"), regions6);
            dataValidate6.CreatePromptBox("错误", "应收金额必须为数值类型");
            sheet1.AddValidationData(dataValidate6);


            var cellHead7 = rowHead.CreateCell(7);  //支付方式
            cellHead7.CellStyle = styleHead;
            cellHead7.SetCellValue(headTitleDesc[7]);
            var regions7 = new CellRangeAddressList(1, 65535, 7, 7);
            var dataValidate7 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(request.PayTypeAll), regions7);
            sheet1.AddValidationData(dataValidate7);

            var cellHead8 = rowHead.CreateCell(8);  //经办日期
            cellHead8.CellStyle = styleHead;
            cellHead8.SetCellValue(headTitleDesc[8]);
            var cellStyle8 = workMbrTemplate.CreateCellStyle();
            var format8 = workMbrTemplate.CreateDataFormat();
            cellStyle8.DataFormat = format8.GetFormat("yyyy-MM-dd");
            sheet1.SetDefaultColumnStyle(8, cellStyle8);

            var cellHead9 = rowHead.CreateCell(9);  //课程类型
            cellHead9.CellStyle = styleHead;
            cellHead9.SetCellValue(headTitleDesc[9]);
            var regions9 = new CellRangeAddressList(1, 65535, 9, 9);
            var dataValidate9 = dvHelper.CreateValidation(dvHelper.CreateExplicitListConstraint(new string[] { "一对多", "一对一" }), regions9);
            sheet1.AddValidationData(dataValidate9);

            using (var fs = File.OpenWrite(request.CheckResult.StrFileFullPath))
            {
                workMbrTemplate.Write(fs);
            }
        }

        public static Tuple<string, List<ImportCourseDayItem>> ReadImportCourseDayExcelContent(Stream excelStream, int sheetIndex, int validDataRowIndex)
        {
            var workbook = WorkbookFactory.Create(excelStream);
            var workSheet = workbook.GetSheetAt(sheetIndex);
            var outStudentContent = new List<ImportCourseDayItem>();
            if (workSheet.LastRowNum <= 1)
            {
                return Tuple.Create("请按要求填写学员信息", outStudentContent);
            }
            if (workSheet.LastRowNum > 1005)
            {
                return Tuple.Create("一次性最多导入1000条数据", outStudentContent);
            }

            var headRow = workSheet.GetRow(validDataRowIndex - 1);
            var headTitleDesc = GetImportCourseHeadDescDay();
            for (var i = 0; i < headTitleDesc.Count; i++)
            {
                if (GetCellValue(headRow.GetCell(i)) != headTitleDesc[i])
                {
                    return Tuple.Create("请选择正确的excel模板文件导入", outStudentContent);
                }
            }

            var strError = new StringBuilder();
            var readRowIndex = validDataRowIndex;
            var now = DateTime.Now;
            while (readRowIndex <= workSheet.LastRowNum)
            {
                if (strError.Length > 200)
                {
                    break;
                }
                var myRow = workSheet.GetRow(readRowIndex);
                if (myRow == null)
                {
                    strError.Append($"第{readRowIndex + 1}行数据无效，请重新检验</br>");
                    continue;
                }
                var myStudentCourseItem = new ImportCourseDayItem();
                var i = myRow.FirstCellNum;

                var studentNameValue = GetCellValue(myRow.GetCell(i));    //学员姓名
                if (string.IsNullOrEmpty(studentNameValue))
                {
                    strError.Append($"第{readRowIndex + 1}行学员姓名不能为空</br>");
                }
                else
                {
                    myStudentCourseItem.StudentName = studentNameValue;
                }

                var phoneCellValue = GetCellValue(myRow.GetCell(++i));   //手机号码
                if (string.IsNullOrEmpty(phoneCellValue) || !EtmsHelper.IsMobilePhone(phoneCellValue))
                {
                    strError.Append($"第{readRowIndex + 1}行手机号码不正确</br>");
                }
                else
                {
                    myStudentCourseItem.Phone = phoneCellValue;
                }

                var courseNameCellValue = GetCellValue(myRow.GetCell(++i));   //课程名称
                if (string.IsNullOrEmpty(courseNameCellValue))
                {
                    strError.Append($"第{readRowIndex + 1}行课程名称不能为空</br>");
                }
                else
                {
                    myStudentCourseItem.CourseName = courseNameCellValue;
                }

                var startTime = GetCellValue(myRow.GetCell(++i));  //课程开始日期
                if (string.IsNullOrEmpty(startTime))
                {
                    strError.Append($"第{readRowIndex + 1}行课程开始日期不能为空</br>");
                }
                else
                {
                    if (DateTime.TryParse(startTime, out DateTime mytempstartTime))
                    {
                        myStudentCourseItem.StartTime = mytempstartTime.Date;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行课程开始日期格式不正确</br>");
                    }
                }


                var endTime = GetCellValue(myRow.GetCell(++i));  //课程结束日期
                if (string.IsNullOrEmpty(endTime))
                {
                    strError.Append($"第{readRowIndex + 1}行课程结束日期不能为空</br>");
                }
                else
                {
                    if (DateTime.TryParse(endTime, out DateTime mytempendTime))
                    {
                        myStudentCourseItem.EndTime = mytempendTime.Date;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行课程结束日期格式不正确</br>");
                    }
                }
                if (myStudentCourseItem.StartTime > myStudentCourseItem.EndTime)
                {
                    strError.Append($"第{readRowIndex + 1}行课程开始日期不能大于结束日期</br>");
                }

                var aptSum = GetCellValue(myRow.GetCell(++i));  //应收金额
                if (string.IsNullOrEmpty(aptSum))
                {
                    strError.Append($"第{readRowIndex + 1}行应收金额不能为空</br>");
                }
                else
                {
                    if (decimal.TryParse(aptSum, out var mytempaptSum))
                    {
                        myStudentCourseItem.AptSum = mytempaptSum;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行应收金额必须为数值类型</br>");
                    }
                }

                var paySum = GetCellValue(myRow.GetCell(++i));  //实收金额
                if (string.IsNullOrEmpty(paySum))
                {
                    strError.Append($"第{readRowIndex + 1}行实收金额不能为空</br>");
                }
                else
                {
                    if (decimal.TryParse(paySum, out var mytemppaySum))
                    {
                        myStudentCourseItem.PaySum = mytemppaySum;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行实收金额必须为数值类型</br>");
                    }
                }

                myStudentCourseItem.PayTypeName = GetCellValue(myRow.GetCell(++i));   //支付方式

                var orderOt = GetCellValue(myRow.GetCell(++i));   //经办日期
                if (string.IsNullOrEmpty(orderOt))
                {
                    myStudentCourseItem.OrderOt = now;
                }
                else
                {
                    if (DateTime.TryParse(orderOt, out DateTime mytemporderOt))
                    {
                        myStudentCourseItem.OrderOt = mytemporderOt;
                    }
                    else
                    {
                        strError.Append($"第{readRowIndex + 1}行经办日期格式不正确</br>");
                    }
                }

                var courseType = GetCellValue(myRow.GetCell(++i));   //经办日期

                if (courseType == "一对一")
                {
                    myStudentCourseItem.CourseType = EmCourseType.OneToOne;
                }

                outStudentContent.Add(myStudentCourseItem);
                readRowIndex++;
            }
            return Tuple.Create(strError.ToString(), outStudentContent);
        }
        #endregion
    }

    public class ImportStudentExcelTemplateRequest
    {
        public CheckImportStudentTemplateFileResult CheckResult { get; set; }

        public List<EtStudentRelationship> StudentRelationshipAll { get; set; }

        public List<EtGrade> GradeAll { get; set; }

        public List<EtStudentSource> StudentSourceAll { get; set; }

    }

    public class ImportCourseHeadDescTimesExcelTemplateRequest
    {
        public CheckImportStudentTemplateFileResult CheckResult { get; set; }

        public string[] PayTypeAll { get; set; }
    }

    public class ImportCourseHeadDescDayExcelTemplateRequest
    {
        public CheckImportStudentTemplateFileResult CheckResult { get; set; }

        public string[] PayTypeAll { get; set; }

    }
}
