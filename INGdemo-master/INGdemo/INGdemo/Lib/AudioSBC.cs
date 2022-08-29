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

    public class SBCDecoder
    {
        sbc_struct  sbc_t;

        int sbc_decode(byte[] data)
        {
            int i, ch, codesize, samples;
            codesize = sbc_unpack_frame(data);


            //初始化

            //解包

            //初始化编码器

            //多相合成

            //输出

            return codesize;
        }


        void sbc_decoder_init()
        {

        }


        int sbc_unpack_frame(byte[] data, int len)
        {
            int consumed;
            byte[] crc_header = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int crc_pos = 0;
            int temp;

            int audio_sample;
            int ch, sb, blk, bit;

            int[,] bits = new int[2,8];
            uint[,] levels = new uint[2,8];

            if (len < 4)
                return -1;

            if (data[0] != Constants.SBC_SYNCWORD)
                return -2;

            sbc_t.priv.frame.frequency = (byte)((data[1] >> 6) & 0x03);

            sbc_t.priv.frame.block_mode = (byte)((data[1] >> 4) & 0x03);
            switch (sbc_t.priv.frame.block_mode) {
            case Constants.SBC_BLK_4:
                sbc_t.priv.frame.blocks = 4;
                break;
            case Constants.SBC_BLK_8:
                sbc_t.priv.frame.blocks = 8;
                break;
            case Constants.SBC_BLK_12:
                sbc_t.priv.frame.blocks = 12;
                break;
            case Constants.SBC_BLK_16:
                sbc_t.priv.frame.blocks = 16;
                break;
            }

            sbc_t.priv.frame.mode = (Channels)((data[1] >> 2) & 0x03); //可能存在问题
            switch (sbc_t.priv.frame.mode) {
            case Channels.MONO:
                sbc_t.priv.frame.channels = 1;
                break;
            case Channels.DUAL_CHANNEL:	/* fall-through */
            case Channels.STEREO:
            case Channels.JOINT_STEREO:
                sbc_t.priv.frame.channels = 2;
                break;
            }

            sbc_t.priv.frame.allocation = (Allocate)((data[1] >> 1) & 0x01);

            sbc_t.priv.frame.subband_mode = (byte)(data[1] & 0x01);
            sbc_t.priv.frame.subbands = (byte)(sbc_t.priv.frame.subband_mode == Constants.SBC_SB_8 ? 8 : 4);

            sbc_t.priv.frame.bitpool = data[2];

            if ((sbc_t.priv.frame.mode == Channels.MONO || sbc_t.priv.frame.mode == Channels.DUAL_CHANNEL) &&
                    sbc_t.priv.frame.bitpool > 16 * sbc_t.priv.frame.subbands)
                return -4;

            if ((sbc_t.priv.frame.mode == Channels.STEREO || sbc_t.priv.frame.mode == Channels.JOINT_STEREO) &&
                    sbc_t.priv.frame.bitpool > 32 * sbc_t.priv.frame.subbands)
                return -4;

            /* data[3] is crc, we're checking it later */

            consumed = 32;

            crc_header[0] = data[1];
            crc_header[1] = data[2];
            crc_pos = 16;

            if (sbc_t.priv.frame.mode == Channels.JOINT_STEREO) {
                if (len * 8 < (uint)(consumed + sbc_t.priv.frame.subbands))
                    return -1;

                sbc_t.priv.frame.joint = 0x00;
                for (sb = 0; sb < sbc_t.priv.frame.subbands - 1; sb++)
                    sbc_t.priv.frame.joint |= (byte)(((data[4] >> (7 - sb)) & 0x01) << sb);
                if (sbc_t.priv.frame.subbands == 4)
                    crc_header[crc_pos / 8] = (byte)(data[4] & 0xf0);
                else
                    crc_header[crc_pos / 8] = data[4];

                consumed += sbc_t.priv.frame.subbands;
                crc_pos += sbc_t.priv.frame.subbands;
            }

            if (len * 8 < (byte)(consumed + (4 * sbc_t.priv.frame.subbands * sbc_t.priv.frame.channels)))
                return -1;

            for (ch = 0; ch < sbc_t.priv.frame.channels; ch++) {
                for (sb = 0; sb < sbc_t.priv.frame.subbands; sb++) {
                    /* FIXME assert(consumed % 4 == 0); */
                    sbc_t.priv.frame.scale_factor[ch,sb] =
                        (uint)(data[consumed >> 3] >> (4 - (consumed & 0x7))) & 0x0F;
                    crc_header[crc_pos >> 3] |=(byte)(
                        sbc_t.priv.frame.scale_factor[ch,sb] << (4 - (crc_pos & (0x7))));

                    consumed += 4;
                    crc_pos += 4;
                }
            }

            if (data[3] != exp.sbc_crc8(crc_header, crc_pos))
                return -3;

            sbc_calculate_bits(bits);

            for (ch = 0; ch < sbc_t.priv.frame.channels; ch++) {
                for (sb = 0; sb < sbc_t.priv.frame.subbands; sb++)
                    levels[ch,sb] = (uint)((1 << bits[ch,sb]) - 1);
            }

            for (blk = 0; blk < sbc_t.priv.frame.blocks; blk++) {
                for (ch = 0; ch < sbc_t.priv.frame.channels; ch++) {
                    for (sb = 0; sb < sbc_t.priv.frame.subbands; sb++) {
                        if (levels[ch,sb] > 0) {
                            audio_sample = 0;
                            for (bit = 0; bit < bits[ch,sb]; bit++) {
                                if (consumed > len * 8)
                                    return -1;

                                if (((data[consumed >> 3] >> (7 - (consumed & 0x7))) & 0x01) != 0)
                                    audio_sample |= 1 << (bits[ch,sb] - bit - 1);

                                consumed++;
                            }

                            sbc_t.priv.frame.sb_sample[blk,ch,sb] =
                                (int)((((audio_sample << 1) | 1) << ((int)sbc_t.priv.frame.scale_factor[ch,sb] + 1)) /
                                levels[ch,sb] - (1 << (int)(sbc_t.priv.frame.scale_factor[ch,sb] + 1)));
                        } else
                            sbc_t.priv.frame.sb_sample[blk,ch,sb] = 0;
                    }
                }
            }

            if (sbc_t.priv.frame.mode == Channels.JOINT_STEREO) {
                for (blk = 0; blk < sbc_t.priv.frame.blocks; blk++) {
                    for (sb = 0; sb < sbc_t.priv.frame.subbands; sb++) {
                        if ((sbc_t.priv.frame.joint & (0x01 << sb)) != 0) {
                            temp = sbc_t.priv.frame.sb_sample[blk,0,sb] +
                                sbc_t.priv.frame.sb_sample[blk,1,sb];
                            sbc_t.priv.frame.sb_sample[blk,1,sb] =
                                sbc_t.priv.frame.sb_sample[blk,0,sb] -
                                sbc_t.priv.frame.sb_sample[blk,1,sb];
                            sbc_t.priv.frame.sb_sample[blk,0,sb] = temp;
                        }
                    }
                }
            }

            if ((consumed & 0x7) != 0)
                consumed += 8 - (consumed & 0x7);

            return consumed >> 3;            

        
        }

        void sbc_calculate_bits(int[,] bits)
        {
            if (sbc_t.priv.frame.subbands == 4)
                sbc_calculate_bits_internal(bits, 4);
            else
                sbc_calculate_bits_internal(bits, 8);
        }

        void sbc_calculate_bits_internal(int[,] bits,int subbands)
        {

        }
        int sbc_get_dec_codesize()
        {
            return 0;
        }


        int sbc_get_dec_frame_length()
        {
            return 0;
        }


        int sbc_synthesize_audio()
        {
            return 0;
        }


     
    }

    public class SBCEncoder
    {

    }

}