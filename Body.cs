using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// Body.cs 추가.
// 이 소스 코드 파일은 네 가지 MSGTYPE에 따른 본문 형식을 각각의 클래스로 나타냅니다.
namespace FUP
{
    public class BodyRequest : ISerializable  // 파일 전송 요청 메시지 본문 클래스, ISerializable 인터페이스 구현
    {
        public long FILESIZE;  // 파일 크기를 저장하는 필드
        public byte[] FILENAME;  // 파일 이름을 저장하는 바이트 배열 필드

        public BodyRequest() { }  // 기본 생성자
        public BodyRequest(byte[] bytes)  // 바이트 배열을 입력으로 받는 생성자
        {
            FILESIZE = BitConverter.ToInt64(bytes, 0);  // 바이트 배열에서 파일 크기를 추출하여 FILESIZE 필드에 저장
            FILENAME = new byte[bytes.Length - sizeof(long)];  // 파일 이름을 저장할 바이트 배열 생성
            Array.Copy(bytes, sizeof(long), FILENAME, 0, FILENAME.Length);  // 바이트 배열에서 파일 이름을 추출하여 FILENAME 필드에 저장
        }

        public byte[] GetBytes()  // BodyRequest 객체를 바이트 배열로 변환하는 메서드
        {
            byte[] bytes = new byte[GetSize()];  // GetSize() 메서드를 사용하여 바이트 배열의 크기를 계산하고 배열 생성
            byte[] temp = BitConverter.GetBytes(FILESIZE);  // FILESIZE를 바이트 배열로 변환하여 temp에 저장
            Array.Copy(temp, 0, bytes, 0, temp.Length);  // temp 배열의 내용을 bytes 배열에 복사
            Array.Copy(FILENAME, 0, bytes, temp.Length, FILENAME.Length);  // FILENAME 배열의 내용을 bytes 배열에 복사

            return bytes;  // bytes 배열 반환
        }

        public int GetSize()  // BodyRequest 객체의 크기를 반환하는 메서드
        {
            return sizeof(long) + FILENAME.Length;  // long 타입의 크기와 FILENAME 배열의 길이를 더하여 반환
        }
    }

    public class BodyResponse : ISerializable  // 파일 전송 요청에 대한 응답 메시지 본문 클래스, ISerializable 인터페이스 구현
    {
        public uint MSGID;  // 메시지 ID를 저장하는 필드
        public byte RESPONSE;  // 응답 결과를 저장하는 필드

        public BodyResponse() { }  // 기본 생성자
        public BodyResponse(byte[] bytes)  // 바이트 배열을 입력으로 받는 생성자
        {
            MSGID = BitConverter.ToUInt32(bytes, 0);  // 바이트 배열에서 메시지 ID를 추출하여 MSGID 필드에 저장
            RESPONSE = bytes[4];  // 바이트 배열에서 응답 결과를 추출하여 RESPONSE 필드에 저장
        }

        public byte[] GetBytes()  // BodyResponse 객체를 바이트 배열로 변환하는 메서드
        {
            byte[] bytes = new byte[GetSize()];  // GetSize() 메서드를 사용하여 바이트 배열의 크기를 계산하고 배열 생성
            byte[] temp = BitConverter.GetBytes(MSGID);  // MSGID를 바이트 배열로 변환하여 temp에 저장
            Array.Copy(temp, 0, bytes, 0, temp.Length);  // temp 배열의 내용을 bytes 배열에 복사
            bytes[temp.Length] = RESPONSE;  // RESPONSE 값을 bytes 배열에 저장

            return bytes;  // bytes 배열 반환
        }

        public int GetSize()  // BodyResponse 객체의 크기를 반환하는 메서드
        {
            return sizeof(uint) + sizeof(byte);  // uint 타입의 크기와 byte 타입의 크기를 더하여 반환
        }
    }

    public class BodyData : ISerializable  // 파일 데이터 전송 메시지 본문 클래스, ISerializable 인터페이스 구현
    {
        public byte[] DATA;  // 파일 데이터를 저장하는 바이트 배열 필드

        public BodyData(byte[] bytes)  // 바이트 배열을 입력으로 받는 생성자
        {
            DATA = new byte[bytes.Length];  // 파일 데이터를 저장할 바이트 배열 생성
            bytes.CopyTo(DATA, 0);  // 입력받은 바이트 배열을 DATA 필드에 복사
        }

        public byte[] GetBytes()  // BodyData 객체를 바이트 배열로 변환하는 메서드
        {
            return DATA;  // DATA 배열 반환
        }

        public int GetSize()  // BodyData 객체의 크기를 반환하는 메서드
        {
            return DATA.Length;  // DATA 배열의 길이를 반환
        }
    }

    public class BodyResult : ISerializable  // 파일 전송 결과 메시지 본문 클래스, ISerializable 인터페이스 구현
    {
        public uint MSGID;  // 메시지 ID를 저장하는 필드
        public byte RESULT;  // 전송 결과를 저장하는 필드

        public BodyResult() { }  // 기본 생성자
        public BodyResult(byte[] bytes)  // 바이트 배열을 입력으로 받는 생성자
        {
            MSGID = BitConverter.ToUInt32(bytes, 0);  // 바이트 배열에서 메시지 ID를 추출하여 MSGID 필드에 저장
            RESULT = bytes[4];  // 바이트 배열에서 전송 결과를 추출하여 RESULT 필드에 저장
        }
        public byte[] GetBytes()  // BodyResult 객체를 바이트 배열로 변환하는 메서드
        {
            byte[] bytes = new byte[GetSize()];  // GetSize() 메서드를 사용하여 바이트 배열의 크기를 계산하고 배열 생성
            byte[] temp = BitConverter.GetBytes(MSGID);  // MSGID를 바이트 배열로 변환하여 temp에 저장
            Array.Copy(temp, 0, bytes, 0, temp.Length);  // temp 배열의 내용을 bytes 배열에 복사
            bytes[temp.Length] = RESULT;  // RESULT 값을 bytes 배열에 저장

            return bytes;  // bytes 배열 반환
        }

        public int GetSize()  // BodyResult 객체의 크기를 반환하는 메서드
        {
            return sizeof(uint) + sizeof(byte);  // uint 타입의 크기와 byte 타입의 크기를 더하여 반환
        }
    }
}