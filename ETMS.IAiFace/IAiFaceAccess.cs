using System;

namespace ETMS.IAiFace
{
    public interface IAiFaceAccess
    {
        void InitTencentCloudConfig(int tenantId, string secretId, string secretKey, string endpoint, string region);

        void StudentDel(long studentId);

        bool StudentInitFace(long studentId, string faceGreyKeyUrl);

        bool StudentClearFace(long studentId);

        long SearchPerson(string imageBase64);
    }
}
