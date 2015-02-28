using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Common {
    /// <summary>
    /// 身份证操作
    /// </summary>
    public class IDCardHelper {
        /// <summary>
        /// 身份证验证
        /// </summary>
        /// <param name="IDCard">身份证号码</param>
        /// <returns>是否真实身份证</returns>
        public static bool CheckIDCard(string IDCard) {
            long n = 0;

            if (IDCard != null && IDCard.Length == 18) {
                if (long.TryParse(IDCard.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(IDCard.Replace('x', '0').Replace('X', '0'), out n) == false) {
                    //数字验证
                    throw new Exception("数字验证失败!");
                }
                const string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(IDCard.Remove(2)) == -1) {
                    //省份验证
                    throw new Exception("省份验证失败!");
                }
                string birth = IDCard.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false) {
                    //生日验证
                    throw new Exception("生日验证失败!");
                }
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                char[] Ai = IDCard.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++) {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);
                if (arrVarifyCode[y] != IDCard.Substring(17, 1).ToLower()) {
                    //校验码验证
                    throw new Exception("校验码验证!");
                }

                return true;//符合GB11643-1999标准
            }
            else if (IDCard != null && IDCard.Length == 15) {
                return CheckIDCard(Per15To18(IDCard));
            }
            else {
                throw new Exception("必须为15位或18位!");
            }
        }

        /// <summary>
        /// 15位身份证号 换算 18位身份号
        /// </summary>
        /// <param name="PerIDSrc">15位身份证号</param>
        /// <returns></returns>
        public static string Per15To18(string PerIDSrc) {
            if (PerIDSrc != null && PerIDSrc.Length == 15) {
                int iS = 0;
                //加权因子常数
                int[] iW = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
                //校验码常数
                const string LastCode = "10X98765432";
                //新身份证号
                string perIDNew = PerIDSrc.Substring(0, 6);
                //填在第6位及第7位上填上‘1’，‘9’两个数字
                perIDNew += "19";
                perIDNew += PerIDSrc.Substring(6, 9);

                //进行加权求和
                for (int i = 0; i < 17; i++) {
                    iS += int.Parse(perIDNew.Substring(i, 1)) * iW[i];
                }

                //取模运算，得到模值
                int iY = iS % 11;
                //从LastCode中取得以模为索引号的值，加到身份证的最后一位，即为新身份证号。
                perIDNew += LastCode.Substring(iY, 1);
                return perIDNew;
            }
            else if (PerIDSrc != null && PerIDSrc.Length == 18) {
                return PerIDSrc;
            }
            else {
                throw new Exception(string.Format("15位（{0}）转换18位失败!", PerIDSrc));
            }
        }


        /// <summary>
        /// 从身份证号码中获取出生日期
        /// </summary>
        /// <param name="IDCard">身份证号码</param>
        /// <returns></returns>
        public static DateTime GetBirthday(string IDCard) {
            IDCard = Per15To18(IDCard);
            if (IDCard != null && CheckIDCard(IDCard)) {
                string dateStr = IDCard.Substring(6, 8).Insert(6, "-").Insert(4, "-");

                return DateTime.Parse(dateStr);
            }
            else {
                return DateTime.MinValue;
            }
        }


        /// <summary>
        /// 从身份证号码中获取性别 2女 1男
        /// </summary>
        /// <param name="IDCard">身份证号码</param>
        /// <returns></returns>
        public static int GetSexInt(string IDCard) {
            IDCard = Per15To18(IDCard);
            int SexZH = 0;
            if (IDCard != null && CheckIDCard(IDCard)) {
                if (int.Parse(IDCard.Substring(16, 1)) % 2 == 0) {
                    SexZH = 2;
                }
                else {
                    SexZH = 1;
                }
            }

            return SexZH;
        }
        /// <summary>
        /// 从身份证号码中获取性别 2女 1男
        /// </summary>
        /// <param name="IDCard">身份证号码</param>
        /// <returns></returns>
        public static string GetSexZH(string IDCard) {
            return GetSexInt(IDCard) == 1 ? "男" : "女";
        }
    }
}
