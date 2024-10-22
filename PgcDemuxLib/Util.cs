using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib {
    internal static class Util {

        public static int GetNbytes(int nNumber, ReadOnlySpan<byte> address) {
            int ret, i;

            for (i = ret = 0; i < nNumber; i++)
            {
                ret = ret * 256 + address[i];
            }

            return ret;
        }

        public static int BCD2Dec(int BCD) {
            return (BCD / 0x10) * 10 + (BCD % 0x10);
        }

        public static int Dec2BCD(int Dec) {
            return (Dec / 10) * 0x10 + (Dec % 10);
        }

        public static ulong AddDuration(ulong dwDuration1, ulong dwDuration2) {
            ulong ret;
            int ifps, hh, mm, ss, ff;
            Int64 i64Dur1, i64Dur2, i64DurT;

            if (((dwDuration1 % 256) & 0x0c0) == 0x0c0)
                ifps = 30;
            else
                ifps = 25;

            i64Dur1 = BCD2Dec((int)((dwDuration1 % 256) & 0x3f));
            i64Dur1 += BCD2Dec((int)((dwDuration1 / 256) % 256)) * ifps;
            i64Dur1 += BCD2Dec((int)((dwDuration1 / (256 * 256)) % 256)) * ifps * 60;
            i64Dur1 += BCD2Dec((int)(dwDuration1 / (256 * 256 * 256))) * ifps * 60 * 60;

            i64Dur2 = BCD2Dec((int)((dwDuration2 % 256) & 0x3f));
            i64Dur2 += BCD2Dec((int)((dwDuration2 / 256) % 256)) * ifps;
            i64Dur2 += BCD2Dec((int)((dwDuration2 / (256 * 256)) % 256)) * ifps * 60;
            i64Dur2 += BCD2Dec((int)(dwDuration2 / (256 * 256 * 256))) * ifps * 60 * 60;

            i64DurT = i64Dur2 + i64Dur1;

            ff = Dec2BCD((int)(i64DurT % ifps));
            ss = Dec2BCD((int)((i64DurT / ifps) % 60));
            mm = Dec2BCD((int)((i64DurT / ifps / 60) % 60));
            hh = Dec2BCD((int)(i64DurT / ifps / 60 / 60));

            ret = (ulong)ff + (ulong)ss * 256 + (ulong)mm * 256 * 256 + (ulong)hh * 256 * 256 * 256;

            if (ifps == 30)
                ret += 0x0c0;
            else
                ret += 0x040;

            return ret;
        }

        public static int readbuffer(Span<byte> caracter, Stream in_) {
            int j;

            if (in_ == null) return -1;
            j = in_.Read(caracter.Slice(0, 2048));

            return j;
        }

        public static bool IsSynch(Span<byte> buffer) {
            int startcode;

            startcode = GetNbytes(4, buffer);

            if (startcode == 0x1BA) return true;
            else return false;
        }

        public static bool IsNav(Span<byte> buffer) {

            int startcode;

            startcode = GetNbytes(4, buffer.Slice(14));

            if (startcode == 443) return true;
            else return false;

        }

        public static void ModifyLBA(Span<byte> buffer, Int64 m_i64OutputLBA) {
            // 1st lba number
            buffer[0x30] = (byte)(m_i64OutputLBA % 256);
            buffer[0x2F] = (byte)((m_i64OutputLBA / 256) % 256);
            buffer[0x2E] = (byte)((m_i64OutputLBA / 256 / 256) % 256);
            buffer[0x2D] = (byte)(m_i64OutputLBA / 256 / 256 / 256);

            // 2nd lba number
            buffer[0x040E] = buffer[0x30];
            buffer[0x040D] = buffer[0x2F];
            buffer[0x040C] = buffer[0x2E];
            buffer[0x040B] = buffer[0x2D];
        }

        public static bool IsAudio(ReadOnlySpan<byte> buffer) {
            int startcode, st_i;

            startcode = GetNbytes(4, buffer.Slice(14));
            st_i = 0x17 + buffer[0x16];
            /*
            0x80-0x87: ac3
            0x88-0x8f: dts
            0x90-0x97: dds
            0x98-0x9f: unknown
            0xa0-0xa7: lpcm

            --------------------------------------------------------------------------------
            SDSS   AC3   DTS   LPCM   MPEG-1   MPEG-2

             90    80    88     A0     C0       D0
             91    81    89     A1     C1       D1
             92    82    8A     A2     C2       D2
             93    83    8B     A3     C3       D3
             94    84    8C     A4     C4       D4
             95    85    8D     A5     C5       D5
             96    86    8E     A6     C6       D6
             97    87    8F     A7     C7       D7
            --------------------------------------------------------------------------------
            */
            if ((startcode == 445 && buffer[st_i] > 0x7f && buffer[st_i] < 0x98) ||
                (startcode == 445 && buffer[st_i] > 0x9f && buffer[st_i] < 0xa8) ||
                (startcode >= 0x1c0 && startcode <= 0x1c7) ||
                (startcode >= 0x1d0 && startcode <= 0x1d7))
                return true;
            else return false;

        }

        public static bool IsSubs(ReadOnlySpan<byte> buffer) {

            int startcode, st_i;

            startcode = GetNbytes(4, buffer.Slice(14));
            st_i = 0x17 + buffer[0x16];


            if (startcode == 445 && buffer[st_i] > 0x1f && buffer[st_i] < 0x40)
                return true;
            else return false;

        }

        public static void writebuffer(ReadOnlySpan<byte> caracter, Stream out_, int nbytes) {
            out_.Write(caracter.Slice(0, nbytes));

            return;
        }

        public static int DurationInFrames(ulong dwDuration) {

            int ifps, ret;
            Int64 i64Dur;

            if (((dwDuration % 256) & 0x0c0) == 0x0c0)
                ifps = 30;
            else
                ifps = 25;

            i64Dur = BCD2Dec((int)((dwDuration % 256) & 0x3f));
            i64Dur += BCD2Dec((int)((dwDuration / 256) % 256)) * ifps;
            i64Dur += BCD2Dec((int)((dwDuration / (256 * 256)) % 256)) * ifps * 60;
            i64Dur += BCD2Dec((int)(dwDuration / (256 * 256 * 256))) * ifps * 60 * 60;

            ret = (int)(i64Dur);

            return ret;
        }

        public static bool IsVideo(ReadOnlySpan<byte> buffer) {

            int startcode;

            startcode = GetNbytes(4, buffer.Slice(14));

            if (startcode == 480) return true;
            else return false;

        }

        public static int readpts(ReadOnlySpan<byte> buf) {
            int a1, a2, a3;
            int pts;

            a1 = (buf[0] & 0xe) >> 1;
            a2 = ((buf[1] << 8) | buf[2]) >> 1;
            a3 = ((buf[3] << 8) | buf[4]) >> 1;
            pts = (int)((((Int64)a1) << 30) | (a2 << 15) | a3);
            return pts;
        }

        public static int getAudId(ReadOnlySpan<byte> buffer) {
            int AudId;


            if (!IsAudio(buffer)) return -1;

            if (IsAudMpeg(buffer))
                AudId = buffer[0x11];
            else
                AudId = buffer[0x17 + buffer[0x16]];

            return AudId;
        }

        public static bool IsAudMpeg(ReadOnlySpan<byte> buffer) {

            int startcode;

            startcode = GetNbytes(4, buffer.Slice(14));

            if ((startcode >= 0x1c0 && startcode <= 0x1c7) ||
                (startcode >= 0x1d0 && startcode <= 0x1d7))
                return true;
            else return false;

        }

        public static bool IsPad(ReadOnlySpan<byte> buffer) {

            int startcode;

            startcode = GetNbytes(4, buffer.Slice(14));

            if (startcode == 446) return true;
            else return false;

        }
    }
}
