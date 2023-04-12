// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("YNJRcmBdVll61hjWp11RUVFVUFPLv2zriw6H3UhAxqb/eDMrmUXO0hHucExmVxb1clnH/Y/7Z5jeO/7moTielJ0WA1iDAmOcjfBzZi1ECUugoh8e+yH6REICT2q6WYyRrs2Uadzd9oI+yhr9BxJ5IIEUGQ5Dlqdk8sefhlg21hqno/aizlw/AgvUXNiPkUOyoWsQOK3eu6ecbDgjv8pXZlObthiDC9787Zuy9CdEkwF4+TSETGEWfqmeeCtJufOd7/xPaePDBZReEb9iMBIlT0tpKBSoL+S41shfV1OyVfl7DJEHB3nvl2FM5K5ccETNj7Vuu9V92bfV3p0Nv2pQ0IOaxzTSUV9QYNJRWlLSUVFQ7p3QY1bNyfELne0dbEZx4VJTUVBR");
        private static int[] order = new int[] { 0,10,2,9,9,7,7,8,9,12,13,11,12,13,14 };
        private static int key = 80;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
