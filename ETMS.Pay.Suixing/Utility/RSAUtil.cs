using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility
{
    public class RSAUtil
    {
        /// <summary>    
        /// RSA公钥格式转换， 
        /// </summary>    
        /// <param name="publicKey">pem公钥</param>    
        /// <returns></returns>    
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
               Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }


        public static void PEMConvertToXML(string strpem, string strxml)//PEM格式密钥转XML
        {
            AsymmetricCipherKeyPair keyPair;
            using (var sr = new StreamReader(strpem))
            {
                var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            }
            var key = (RsaPrivateCrtKeyParameters)keyPair.Private;
            var p = new RSAParameters
            {
                Modulus = key.Modulus.ToByteArrayUnsigned(),
                Exponent = key.PublicExponent.ToByteArrayUnsigned(),
                D = key.Exponent.ToByteArrayUnsigned(),
                P = key.P.ToByteArrayUnsigned(),
                Q = key.Q.ToByteArrayUnsigned(),
                DP = key.DP.ToByteArrayUnsigned(),
                DQ = key.DQ.ToByteArrayUnsigned(),
                InverseQ = key.QInv.ToByteArrayUnsigned(),
            };
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(p);
            using (var sw = new StreamWriter(strxml))
            {
                sw.Write(rsa.ToXmlString(true));
            }
        }

        public static void XMLConvertToPEM(string strpem, string strxml)//XML格式密钥转PEM
        {
            var rsa2 = new RSACryptoServiceProvider();
            using (var sr = new StreamReader(strxml))
            {
                rsa2.FromXmlString(sr.ReadToEnd());
            }
            var p = rsa2.ExportParameters(true);

            var key = new RsaPrivateCrtKeyParameters(
                new BigInteger(1, p.Modulus), new BigInteger(1, p.Exponent), new BigInteger(1, p.D),
                new BigInteger(1, p.P), new BigInteger(1, p.Q), new BigInteger(1, p.DP), new BigInteger(1, p.DQ),
                new BigInteger(1, p.InverseQ));

            using (var sw = new StreamWriter(strpem))
            {
                var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                pemWriter.WriteObject(key);
            }
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="str">待验证的字符串</param>
        /// <param name="sign">加签之后的字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>签名是否符合</returns>
        public static bool SignCheck(string str, string sign, string publicKey)
        {
            try
            {
                byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(str);
                // byte[] bt = Encoding.GetEncoding(encoding).GetBytes(str);
                var sha256 = new SHA256CryptoServiceProvider();
                byte[] rgbHash = sha256.ComputeHash(bt);

                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(publicKey);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
                deformatter.SetHashAlgorithm("SHA256");
                byte[] rgbSignature = Convert.FromBase64String(sign);
                if (deformatter.VerifySignature(rgbHash, rgbSignature))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="SignedData">Base64 签名</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static bool VerifySigned(string plaintext, string SignedData, string publicKey)
        {
            try
            {
                using (RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider())
                {
                    RSAalg.FromXmlString(publicKey);
                    UTF8Encoding ByteConverter = new UTF8Encoding();
                    byte[] dataToVerifyBytes = ByteConverter.GetBytes(plaintext);
                    byte[] signedDataBytes = Convert.FromBase64String(SignedData);
                    return RSAalg.VerifyData(dataToVerifyBytes, new SHA1CryptoServiceProvider(), signedDataBytes);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 验签函数
        /// </summary>
        /// <param name="content"></param>
        /// <param name="signedString"></param>
        /// <param name="publicKey"></param>
        /// <param name="inputCharset"></param>
        /// <returns></returns>
        public static bool Verify(string content, string signedString, string publicKey, string inputCharset)
        {
            bool result = false;

            Encoding code = Encoding.GetEncoding(inputCharset);
            byte[] data = code.GetBytes(content);
            byte[] soureData = Convert.FromBase64String(signedString);
            RSAParameters paraPub = ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.ImportParameters(paraPub);
            SHA1 sh = new SHA1CryptoServiceProvider();
            result = rsaPub.VerifyData(data, sh, soureData);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pemFileConent"></param>
        /// <returns></returns>
        internal static RSAParameters ConvertFromPublicKey(string pempublicKey)
        {
            byte[] keyData = Convert.FromBase64String(pempublicKey);
            //if (keyData.Length < 162)
            //{
            //    throw new ArgumentException("pem file content is incorrect.");
            //}
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);  //将2进制编码转换为8位无符号整数数组. 
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);  //将指定字节数组中的一个字节序列解码为一个字符串。 
            }
            catch
            {
                decode = code;
            }
            return decode;
        }



        //验证签名
        public static bool VerifySignedHash(string str_DataToVerify, string str_SignedData, string str_Public_Key)
        {
            byte[] SignedData = Convert.FromBase64String(str_SignedData);

            ASCIIEncoding ByteConverter = new ASCIIEncoding();
            byte[] DataToVerify = ByteConverter.GetBytes(str_DataToVerify);
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportCspBlob(Convert.FromBase64String(str_Public_Key));

                return RSAalg.VerifyData(DataToVerify, new SHA1CryptoServiceProvider(), SignedData);

            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="OriginalString"></param>
        /// <param name="SignatureString"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static bool Verify(String OriginalString, String SignatureString, String publicKey)
        {
            try
            {
                // 将base64签名数据转码为字节
                byte[] signedBase64 = Convert.FromBase64String(SignatureString);
                byte[] orgin = Encoding.UTF8.GetBytes(OriginalString);

                RSACryptoServiceProvider oRSA = new RSACryptoServiceProvider();
                oRSA.FromXmlString(RSAPublicKeyJava2DotNet(publicKey));

                bool bVerify = oRSA.VerifyData(orgin, "SHA1", signedBase64);
                return bVerify;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string RSASign(string data, string privateKeyPem)
        {
            RSACryptoServiceProvider rsaCsp = LoadCertificateString(privateKeyPem);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
            return Convert.ToBase64String(signatureBytes);
        }

        private static RSACryptoServiceProvider LoadCertificateString(string strKey)
        {
            var data = Convert.FromBase64String(strKey);
            RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(data);
            return rsa;
        }

        private static byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format("-----BEGIN {0}-----\\n", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }

        private static RSACryptoServiceProvider LoadCertificateFile(string filename)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }
                try
                {
                    RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(res);
                    return rsa;
                }
                catch (Exception ex)
                {
                }
                return null;
            }
        }


        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)        //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();    // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;        // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {    //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);        //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        public static string GetSignContent(IDictionary<string, object> parameters)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, object> sortedParams = new SortedDictionary<string, object>(parameters);
            IEnumerator<KeyValuePair<string, object>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                object value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && value != null)
                {
                    if (key == "respData")
                    {
                        var respData = JsonHelper.ObjectToJSON(value);
                        var list = JsonHelper.RowFromJSON(respData);
                        value = DictionaryToStr(list);
                        query.Append(key).Append("=").Append(value).Append("&");
                    }
                    else
                    {
                        query.Append(key).Append("=").Append(value).Append("&");
                    }
                }
            }
            string content = query.ToString().Substring(0, query.Length - 1);

            return replaceStr(content);
        }


        /// <summary>
        /// 自己拼接一遍
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string DictionaryToStr(IDictionary<string, string> dic)
        {
            //使用排序字典
            //   dic = new SortedDictionary<string, string>(dic);
            string strTemp = "{";
            foreach (KeyValuePair<string, string> item in dic)
            {
                if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                {
                    strTemp += string.Format("\"{0}\":\"{1}\"", item.Key, item.Value);
                    strTemp += ",";
                }
            }
            strTemp = strTemp.ToString().Substring(0, strTemp.Length - 1);
            strTemp += "}";
            return strTemp;

        }


        public static string replaceStr(string b)
        {

            b = b.Replace("\r", ""); //删除百度\r
            b = b.Replace("\n", "");//删除\n
            b = b.Replace("\t", "");//删除\t
            //b = b.Replace(" ", "");//删除空格
            return b;
        }
    }
}
