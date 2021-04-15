using System;

namespace ETMS.IAiFace
{
    public interface IAiTenantFaceAccess
    {
        void InitTencentCloudConfig(int tenantId, string secretId, string secretKey, string endpoint, string region);

        void StudentDel(long studentId);

        Tuple<bool, string> StudentInitFace(long studentId, string faceGreyKeyUrl);

        bool StudentClearFace(long studentId);

        Tuple<long, string> SearchPerson(string imageBase64);
    }
}
