using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace INGdemo.Lib
{   
    public class SBCProtcol 
    {
        
        static public Int32 CI(UInt32 i)
        {
            return Convert.ToInt32(i);
        }

        static public Int32 SS4(Int32 i)
        {
            return i >> 12;
        }

        static public Int32 SN4(Int32 i)
        {
            return  i >> 11; 
        }

        static public Int32 SN8(Int32 i)
        {
            return  i >> 11; 
        }

        public 
        static readonly Int32[] sbcProto4sub40m0 = {
            SS4(0x00000000),     SS4(CI(0xffa6982f)), SS4(CI(0xfba93848)), SS4(0x0456c7b8),
	        SS4(0x005967d1),     SS4(CI(0xfffb9ac7)), SS4(CI(0xff589157)), SS4(CI(0xf9c2a8d8)),
	        SS4(0x027c1434),     SS4(0x0019118b),     SS4(CI(0xfff3c74c)), SS4(CI(0xff137330)),
	        SS4(CI(0xf81b8d70)), SS4(0x00ec1b8b),     SS4(CI(0xfff0b71a)), SS4(CI(0xffe99b00)),
	        SS4(CI(0xfef84470)), SS4(CI(0xf6fb4370)), SS4(CI(0xffcdc351)), SS4(CI(0xffe01dc7))
        };

        static readonly  Int32[] sbcProto4sub40m1 = {
        	SS4(CI(0xffe090ce)), SS4(CI(0xff2c0475)), SS4(CI(0xf694f800)), SS4(CI(0xff2c0475)),
	        SS4(CI(0xffe090ce)), SS4(CI(0xffe01dc7)), SS4(CI(0xffcdc351)), SS4(CI(0xf6fb4370)),
	        SS4(CI(0xfef84470)), SS4(CI(0xffe99b00)), SS4(CI(0xfff0b71a)), SS4(0x00ec1b8b),
	        SS4(CI(0xf81b8d70)), SS4(CI(0xff137330)), SS4(CI(0xfff3c74c)), SS4(0x0019118b),
	        SS4(0x027c1434),     SS4(CI(0xf9c2a8d8)), SS4(CI(0xff589157)), SS4(CI(0xfffb9ac7))
        };

        static readonly Int32[,] synmatrix4 = {
            { SN4(0x05a82798),     SN4(CI(0xfa57d868)), SN4(CI(0xfa57d868)), SN4(0x05a82798) },
	        { SN4(0x030fbc54),     SN4(CI(0xf89be510)), SN4(0x07641af0),     SN4(CI(0xfcf043ac)) },
	        { SN4(0x00000000),     SN4(0x00000000),     SN4(0x00000000),     SN4(0x00000000) },
	        { SN4(CI(0xfcf043ac)), SN4(0x07641af0),     SN4(CI(0xf89be510)), SN4(0x030fbc54) },
	        { SN4(CI(0xfa57d868)), SN4(0x05a82798),     SN4(0x05a82798),     SN4(CI(0xfa57d868)) },
	        { SN4(CI(0xf89be510)), SN4(CI(0xfcf043ac)), SN4(0x030fbc54),     SN4(0x07641af0) },
	        { SN4(CI(0xf8000000)), SN4(CI(0xf8000000)), SN4(CI(0xf8000000)), SN4(CI(0xf8000000)) },
	        { SN4(CI(0xf89be510)), SN4(CI(0xfcf043ac)), SN4(0x030fbc54),     SN4(0x07641af0) }
        };

        static readonly Int32[] synmatrix8 = {
            SN8(0x05a82798), SN8(0x0471ced0), SN8(0x07d8a5f0), SN8(0x018f8b84), 
            SN8(0x06a6d988), SN8(0x030fbc54), SN8(0x07641af0), SN8(CI(0xf8000000))
        };
    }

 
}