1.安装服务
New-Service -Name ETMSDaemonService -BinaryPathName E:\ETMS_Running\DaemonService\ETMS.DaemonService.exe -Description "教培系统守护程序" -DisplayName "ETMSDaemonService" -StartupType Automatic

2.检查服务状态
Get-Service ETMSDaemonService

3.启动服务
Start-Service ETMSDaemonService

4.停止服务
Stop-Service ETMSDaemonService

5.卸载删除服务
sc.exe delete ETMSDaemonService
