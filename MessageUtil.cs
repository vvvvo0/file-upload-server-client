using System;
using System.IO;


// MessageUtil.cs 추가.
// 이 소스 코드 파일은 스트림으로부터 메시지를 보내고 받기 위한 메서드를 가지는
// MessageUtil 클래스를 구현합니다.
namespace FUP
{
    public class MessageUtil
    {
        public static void Send(Stream writer, Message msg) // Send() 메서드:
                                                            // 스트림을 통해 메시지를 내보냅니다.
                                                            // 메시지를 스트림에 쓰는 메서드
        {
            writer.Write(msg.GetBytes(), 0, msg.GetSize()); // msg를 바이트 배열로 변환하여 스트림에 씁니다.
        }
        public static Message Receive(Stream reader) // 스트림에서 메시지를 읽는 메서드
        {
            int totalRecv = 0; // 현재까지 수신된 바이트 수
            int sizeToRead = 16; // 읽어야 할 바이트 수 (헤더 크기)
            byte[] hBuffer = new byte[sizeToRead]; // 헤더 데이터를 저장할 바이트 배열

            while (sizeToRead > 0) // 헤더를 모두 읽을 때까지 반복
            {
                byte[] buffer = new byte[sizeToRead]; // 읽어올 데이터를 저장할 임시 바이트 배열
                int recv = reader.Read(buffer, 0, sizeToRead); // 스트림에서 sizeToRead 바이트만큼 읽어서 buffer에 저장
                if (recv == 0) // 스트림에서 더 이상 읽을 데이터가 없으면 null 반환
                    return null;

                buffer.CopyTo(hBuffer, totalRecv); // 읽어온 데이터를 hBuffer에 복사
                totalRecv += recv; // 수신된 바이트 수 업데이트
                sizeToRead -= recv; // 남은 읽어야 할 바이트 수 업데이트
            }

            Header header = new Header(hBuffer); // 헤더 데이터를 사용하여 Header 객체 생성

            totalRecv = 0; // 현재까지 수신된 바이트 수 초기화
            byte[] bBuffer = new byte[header.BODYLEN]; // 본문 데이터를 저장할 바이트 배열
            sizeToRead = (int)header.BODYLEN; // 읽어야 할 바이트 수 (본문 크기)

            while (sizeToRead > 0) // 본문을 모두 읽을 때까지 반복
            {
                byte[] buffer = new byte[sizeToRead]; // 읽어올 데이터를 저장할 임시 바이트 배열
                int recv = reader.Read(buffer, 0, sizeToRead); // 스트림에서 sizeToRead 바이트만큼 읽어서 buffer에 저장
                if (recv == 0) // 스트림에서 더 이상 읽을 데이터가 없으면 null 반환
                    return null;

                buffer.CopyTo(bBuffer, totalRecv); // 읽어온 데이터를 bBuffer에 복사
                totalRecv += recv; // 수신된 바이트 수 업데이트
                sizeToRead -= recv; // 남은 읽어야 할 바이트 수 업데이트
            }

            ISerializable body = null; // 메시지 본문을 저장할 ISerializable 객체

            switch (header.MSGTYPE) // 메시지 유형에 따라 본문 객체 생성
                                    // (헤더의 MSGTYPE 프로퍼티를 통해,
                                    // 어떤 Body 클래스의 생성자를 호출할 지 결정합니다.)
            {
                case CONSTANTS.REQ_FILE_SEND:
                    body = new BodyRequest(bBuffer);
                    break;
                case CONSTANTS.REP_FILE_SEND:
                    body = new BodyResponse(bBuffer);
                    break;
                case CONSTANTS.FILE_SEND_DATA:
                    body = new BodyData(bBuffer);
                    break;
                case CONSTANTS.FILE_SEND_RES:
                    body = new BodyResult(bBuffer);
                    break;
                default: // 알 수 없는 메시지 유형인 경우 예외 발생
                    throw new Exception(
                        String.Format(
                        "Unknown MSGTYPE : {0}" + header.MSGTYPE));
            }

            return new Message() { Header = header, Body = body }; // Header와 Body를 포함하는 Message 객체 생성 및 반환
        }
    }
}