using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;


/*
파일 업로드 서버와 클라이언트 구현은 총 세 단계로 나눠서 진행할 것입니다.
(1) 서버/클라이언트가 같이 사용할 '클래스 라이브러리' 구현
(2) 파일 업로드 '서버' 구현
(3) '클라이언트' 구현


클래스 라이브러리 구현은 일반 응용 프로그램을 만드는 것과 특별히 다른 부분은 없습니다.
빌드한 결과물 파일의 확장자가 exe가 아닌 dll이라는 것과, 
혼자서는 실행되지 않는다는 점이 다를 뿐입니다.


파일 업로드 서버와 클라이언트는 모두 FUP 프로토콜을 사용합니다.
즉, FUP 프로토콜을 처리하는 코드를 서버와 클라이언트 양쪽에서 공유할 수 있습니다.
따라서 'FUP 프로토콜'을 '클래스 라이브러리'로 만들어 놓는 것이 첫 번째 단계입니다.


파일 업로드 프로토콜(FUP: File Upload Protocol):
헤더와 바디 부분으로 나뉩니다.
'바디'에 실제로 전달하려는 데이터를 담고,
'헤더'에는 본문 길이를 비롯해서 메시지의 속성 몇 가지를 담습니다.
'바디의 길이'는 담는 데이터에 따라 달라지지만, 
'헤더의 길이'는 16바이트로 항상 일정합니다.
따라서 수신한 패킷을 분석할 때는 가장 먼저 16바이트를 먼저 확인해서,
바디의 길이와 메시지의 속성을 확인하고,
그 다음에 바디의 길이만큼을 읽어 하나의 메시지 끝을 찾아 끊어내야 합니다.


고정 길이 vs 가변 길이
스트림에서 패킷의 경계를 구분하는 방법입니다.
'고정 길이 형식': 모든 메시지가 같은 길이를 갖습니다. 
16바이트면 16바이트씩, 32바이트면 32바이트씩만 항상 잘라냅니다.
'가변 길이 형식': 
메시지를 두 부분으로 나눠서 길이가 고정된 앞부분에 뒷부분의 길이를 기입하는 방식(주로 바이너리 통신에 이용)과,
메시지를 구분하는 특정 값(''나 캐리지 리턴 등)을 이용하는 방식(주로 텍스트 방식의 통신에 이용)이 있습니다.
 */


// 서버/클라이언트가 같이 사용할 '클래스 라이브러리' 구현
// ('파일 업로드 프로토콜(FUP)'을 '클래스 라이브러리'로 만들어 놓기)
namespace FUP
{
    public class CONSTANTS // 네트워크 통신에 사용될 상수들을 정의하는 클래스
    {
        public const uint REQ_FILE_SEND = 0x01; // 파일 전송 요청 메시지 타입
        public const uint REP_FILE_SEND = 0x02; // 파일 전송 응답 메시지 타입
        public const uint FILE_SEND_DATA = 0x03; // 파일 데이터 전송 메시지 타입
        public const uint FILE_SEND_RES = 0x04; // 파일 전송 결과 메시지 타입


        public const byte NOT_FRAGMENTED = 0x00; // 단편화되지 않은 메시지
        public const byte FRAGMENTED = 0x01; // 단편화된 메시지

        public const byte NOT_LASTMSG = 0x00; // 마지막 메시지가 아님
        public const byte LASTMSG = 0x01; // 마지막 메시지


        public const byte ACCEPTED = 0x00; // 요청 수락됨
        public const byte DENIED = 0x01; // 요청 거부됨


        public const byte FAIL = 0x00; // 실패
        public const byte SUCCESS = 0x01; // 성공
    }


    public interface ISerializable // 직렬화 인터페이스
                                   // 메시지, 헤더, 바디 모두 ISerializable 인터페이스를 상속합니다.
                                   // 따라서 메시지, 헤더, 바디는
                                   // 자신의 데이터를 바이트 배열로 반환하고,
                                   // 그 바이트 배열의 크기를 반환해야 합니다.
    {
        byte[] GetBytes(); // 객체를 바이트 배열로 변환하는 메서드

        int GetSize(); // 객체의 크기를 반환하는 메서드
    }

    public class Message : ISerializable // ISerializable 인터페이스를 구현하는 Message 클래스
                                         // 파일 업로드 프로토콜(FUP)의 메시지를 나타내는 클래스로,
                                         // Header와 Body로 구성됩니다.
    {
        public Header Header { get; set; } // 메시지 헤더
        public ISerializable Body { get; set; } // 메시지 본문

        public byte[] GetBytes() // 객체를 바이트 배열로 변환하는 메서드
        {
            byte[] bytes = new byte[GetSize()]; // 메시지 크기만큼의 바이트 배열 생성

            Header.GetBytes().CopyTo(bytes, 0); // 헤더를 바이트 배열에 복사
            Body.GetBytes().CopyTo(bytes, Header.GetSize()); // 본문을 바이트 배열에 복사 (헤더 다음 위치부터)

            return bytes; // 바이트 배열 반환
        }

        public int GetSize() // 객체의 크기를 반환하는 메서드
        {
            return Header.GetSize() + Body.GetSize(); // 헤더 크기와 본문 크기를 더하여 반환
        }
    }
}
