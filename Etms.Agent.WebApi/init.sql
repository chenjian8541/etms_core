GO
--[EtGrade]
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'幼儿园', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'学前班', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'一年级', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'二年级', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'三年级', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'四年级', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'五年级', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'六年级', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'初一', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'初二', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'初三', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'高一', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'高二', N'', 0);
INSERT [dbo].[EtGrade] ([TenantId],
                        [Name],
                        [Remark],
                        [IsDeleted])
VALUES ('{0}', N'高三', N'', 0);
GO
--[EtIncomeProjectType]
INSERT [EtIncomeProjectType] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'水费', N'', 0);
INSERT [EtIncomeProjectType] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'电费', N'', 0);
INSERT [EtIncomeProjectType] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'房租', N'', 0);
INSERT [EtIncomeProjectType] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'物业费', N'', 0);
INSERT [EtIncomeProjectType] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'活动费', N'', 0);
INSERT [EtIncomeProjectType] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'工资结算', N'', 0);
GO
--[EtStudentGrowingTag]
INSERT [EtStudentGrowingTag] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'节日活动', N'', 0);
INSERT [EtStudentGrowingTag] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'作品展示', N'', 0);
INSERT [EtStudentGrowingTag] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'学员风采', N'', 0);
INSERT [EtStudentGrowingTag] ([TenantId],
                              [Name],
                              [Remark],
                              [IsDeleted])
VALUES ('{0}', N'活力课堂', N'', 0);
GO
--[EtStudentRelationship]
INSERT [EtStudentRelationship] ([TenantId],
                                [Name],
                                [Remark],
                                [IsDeleted])
VALUES ('{0}', N'妈妈', N'', 0);
INSERT [EtStudentRelationship] ([TenantId],
                                [Name],
                                [Remark],
                                [IsDeleted])
VALUES ('{0}', N'爸爸', N'', 0);
INSERT [EtStudentRelationship] ([TenantId],
                                [Name],
                                [Remark],
                                [IsDeleted])
VALUES ('{0}', N'爷爷', N'', 0);
INSERT [EtStudentRelationship] ([TenantId],
                                [Name],
                                [Remark],
                                [IsDeleted])
VALUES ('{0}', N'奶奶', N'', 0);
INSERT [EtStudentRelationship] ([TenantId],
                                [Name],
                                [Remark],
                                [IsDeleted])
VALUES ('{0}', N'外公', N'', 0);
INSERT [EtStudentRelationship] ([TenantId],
                                [Name],
                                [Remark],
                                [IsDeleted])
VALUES ('{0}', N'外婆', N'', 0);
INSERT [EtStudentRelationship] ([TenantId],
                                [Name],
                                [Remark],
                                [IsDeleted])
VALUES ('{0}', N'哥哥', N'', 0);
INSERT [EtStudentRelationship] ([TenantId],
                                [Name],
                                [Remark],
                                [IsDeleted])
VALUES ('{0}', N'姐姐', N'', 0);
GO
--[EtStudentSource]
INSERT [EtStudentSource] ([TenantId],
                          [Name],
                          [Remark],
                          [IsDeleted])
VALUES ('{0}', N'家长分享', N'', 0);
INSERT [EtStudentSource] ([TenantId],
                          [Name],
                          [Remark],
                          [IsDeleted])
VALUES ('{0}', N'地推活动', N'', 0);
INSERT [EtStudentSource] ([TenantId],
                          [Name],
                          [Remark],
                          [IsDeleted])
VALUES ('{0}', N'转介绍', N'', 0);
INSERT [EtStudentSource] ([TenantId],
                          [Name],
                          [Remark],
                          [IsDeleted])
VALUES ('{0}', N'门店到访', N'', 0);
INSERT [EtStudentSource] ([TenantId],
                          [Name],
                          [Remark],
                          [IsDeleted])
VALUES ('{0}', N'电话邀约', N'', 0);
GO
--[EtSubject]
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'语文', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'数学', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'英语', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'历史', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'地理', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'生物', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'物理', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'化学', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'声乐', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'乐器', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'舞蹈', NULL, 0);
INSERT [EtSubject] ([TenantId],
                    [Name],
                    [Remark],
                    [IsDeleted])
VALUES ('{0}', N'跆拳道', NULL, 0);
GO
--[EtRole]
INSERT [EtRole] ([TenantId],
                 [Name],
                 [AuthorityValueMenu],
                 [AuthorityValueData],
                 [Remark],
                 [IsDeleted])
VALUES ('{0}', N'校长', N'4719646399775512298052911102|590295810358705651710|4719646399775512298052911102', N'0', N'校长', 0);
GO
--[EtUser]
INSERT [EtUser] ([TenantId],
                 [RoleId],
                 [Name],
                 [Phone],
                 [Password],
                 [Address],
                 [IsTeacher],
                 [Avatar],
                 [NickName],
                 [Gender],
                 [SubjectsGoodAt],
                 [JobType],
                 [JobAddTime],
                 [TeacherCertification],
                 [TotalClassCount],
                 [TotalClassTimes],
                 [WechatUnionid],
                 [WechatOpenid],
                 [LastLoginTime],
                 [ConfigInfo],
                 [Remark],
                 [IsAdmin],
                 [IsDeleted])
VALUES ('{0}', (SELECT TOP 1 Id FROM EtRole WHERE TenantId = '{0}'), N'{2}', N'{3}', N'1zU2c+OVLS2oIG6BqOyw2g==', N'',
        1, NULL, '', 0, N'', 0, GETDATE(), N'', 0, 0, N'', N'', NULL, N'', N'', 1, 0);