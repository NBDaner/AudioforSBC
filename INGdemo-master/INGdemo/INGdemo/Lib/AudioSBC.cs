using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace INGdemo.Lib
{   

    //结构体
    //枚举
    //构造函数
    //析构函数
    
    public interface ISBCAudio
    {
        bool Write(Int16[] samples);
        void Play(int samplingRate);
        void Stop();
    }   

    class SBCDecoderState
    {
        internal int subband;
        internal Int32[,] V = new Int32[2,170];
        internal int[,] offset = new int[2,16];
    }

    public class SBCDecoder
    {
        internal 
    }

    public class SBCEncoder
    {

    }

}