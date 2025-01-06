using System;
using System.Collections.Generic;
//  제네릭 컬렉션 클래스를 포함합니다. List<T>, Dictionary<TKey, TValue> 등과 같은
//  제네릭 컬렉션을 사용하려면 이 네임스페이스를 사용해야 합니다. 
using System.Linq;
// LINQ (Language Integrated Query) 기능을 사용하기 위한 클래스를 포함합니다. 
// LINQ는 데이터를 쿼리하는 데 사용되는 강력한 기능입니다.
using System.Text;
// 문자열 처리와 인코딩을 위한 클래스를 포함합니다.
//  StringBuilder, Encoding 등과 같은 클래스들이 여기에 속합니다.
using System.Threading.Tasks;
// 비동기 작업을 위한 클래스를 포함합니다.
// Task, Task<TResult> 등과 같은 클래스들이 여기에 속합니다.


// FUP 프로젝트에 Header.cs를 추가 
namespace FUP
{
    public class Header : ISerializable  // Header 클래스는 ISerializable 인터페이스를 구현합니다.
    {
        public uint MSGID { get; set; }  // 메시지 ID를 저장하는 프로퍼티입니다.
        public uint MSGTYPE { get; set; }  // 메시지 유형을 저장하는 프로퍼티입니다.
        public uint BODYLEN { get; set; }  // 메시지 본문의 길이를 저장하는 프로퍼티입니다.
        public byte FRAGMENTED { get; set; }  // 메시지가 조각화되었는지 여부를 저장하는 프로퍼티입니다.
        public byte LASTMSG { get; set; }  // 메시지가 마지막 메시지인지 여부를 저장하는 프로퍼티입니다.
        public ushort SEQ { get; set; }  // 메시지 순서를 저장하는 프로퍼티입니다.

        public Header() { }  // 매개변수가 없는 생성자입니다.
        public Header(byte[] bytes)  // 바이트 배열을 입력으로 받는 생성자입니다.
        {
            MSGID = BitConverter.ToUInt32(bytes, 0);  // 바이트 배열에서 메시지 ID를 추출합니다.
            MSGTYPE = BitConverter.ToUInt32(bytes, 4);  // 바이트 배열에서 메시지 유형을 추출합니다.
            BODYLEN = BitConverter.ToUInt32(bytes, 8);  // 바이트 배열에서 메시지 본문의 길이를 추출합니다.
            FRAGMENTED = bytes[12];  // 바이트 배열에서 메시지 조각화 여부를 추출합니다.
            LASTMSG = bytes[13];  // 바이트 배열에서 메시지가 마지막 메시지인지 여부를 추출합니다.
            SEQ = BitConverter.ToUInt16(bytes, 14);  // 바이트 배열에서 메시지 순서를 추출합니다.
        }

        public byte[] GetBytes()  // GetBytes() 메서드는 Header 객체를 바이트 배열로 변환합니다.
        {
            byte[] bytes = new byte[16];  // 16바이트 크기의 바이트 배열을 생성합니다.

            byte[] temp = BitConverter.GetBytes(MSGID);  // MSGID를 바이트 배열로 변환합니다.
            Array.Copy(temp, 0, bytes, 0, temp.Length);  // temp 배열의 내용을 bytes 배열에 복사합니다.

            temp = BitConverter.GetBytes(MSGTYPE);  // MSGTYPE을 바이트 배열로 변환합니다.
            Array.Copy(temp, 0, bytes, 4, temp.Length);  // temp 배열의 내용을 bytes 배열에 복사합니다.

            temp = BitConverter.GetBytes(BODYLEN);  // BODYLEN을 바이트 배열로 변환합니다.
            Array.Copy(temp, 0, bytes, 8, temp.Length);  // temp 배열의 내용을 bytes 배열에 복사합니다.

            bytes[12] = FRAGMENTED;  // FRAGMENTED 값을 bytes 배열에 저장합니다.
            bytes[13] = LASTMSG;  // LASTMSG 값을 bytes 배열에 저장합니다.

            temp = BitConverter.GetBytes(SEQ);  // SEQ를 바이트 배열로 변환합니다.
            Array.Copy(temp, 0, bytes, 14, temp.Length);  // temp 배열의 내용을 bytes 배열에 복사합니다.

            return bytes;  // bytes 배열을 반환합니다.
        }

        public int GetSize()  // GetSize() 메서드는 Header 객체의 크기를 반환합니다.
        {
            return 16;  // Header 객체의 크기는 16바이트입니다.
        }
    }
}

