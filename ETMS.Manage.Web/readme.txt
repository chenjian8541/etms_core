1.安装服务
New-Service -Name ETMSJobService -BinaryPathName E:\ETMS_Running\WebManage\ETMS.Manage.Web.exe -Description "教培系统Job程序" -DisplayName "ETMSJobService" -StartupType Automatic

2.检查服务状态
Get-Service ETMSJobService

3.启动服务
Start-Service ETMSJobService

4.停止服务
Stop-Service ETMSJobService

5.卸载删除服务
sc.exe delete ETMSJobService
