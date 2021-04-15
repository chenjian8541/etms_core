using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IAiFace
{
    public interface IAiBaiduFaceAccess
    {
        void InitBaiduCloudConfig(int tenantId, string appid, string apiKey, string secretKey);

        void StudentDel(long studentId);

        Tuple<bool, string> StudentInitFace(long studentId, string faceGreyKeyUrl);

        bool StudentClearFace(long studentId);

        Tuple<long, string> SearchPerson(string imageBase64);
    }
}
