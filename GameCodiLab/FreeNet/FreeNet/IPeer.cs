﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FreeNet
{
    /// <summary>
    /// 서버와 클라이언트에서 공통으로 사용하는 세션 객체
    /// 서버일 경우 :
    ///     하나의 클라이언트 객체를 나타낸다.
    ///     이 인터페이스를 구현한 객체를 CNetworkService클래스의 
    ///     session_createed_callback 호출시 생성하여 리턴시켜 준다.
    ///     객체를 풀링할지 여부는 사용자가 원하는대로 구현합니다.
    ///     
    /// 클라이언트일 경우 :
    ///     접속한 서버 객체를 나타낸다.
    ///     
    /// 이 클래스의 모든 매소드는 thread unsafe하므로 공유자원에 접근할 떄는 동기화처리가 필요합니다. 
    /// </summary>
    public interface IPeer
    {
        /// <summary>
        /// 소켓 버퍼로부터 데이터를 수신하여 패킷 하나를 완성했을 때 호출 됩니다.
        /// 호출 흐름 : .Net Socket ReceiveAsync -> CUserToken.on_receive -> CPeer.on_message
        /// 
        /// 패킷 순서에 대해서 (TCP)
        ///     이 매소드는 .Net Socket의 스레드 풀에 의해 작동되며 호출되므로 어느
        ///     스레드에서 호출될지 알수 없습니다.
        ///     하지만 하나의 CPeer객체에 대해서는 이 매소드가 완료된 이후 다음 패킷이 들어오도록 구현되어 있으므로
        ///     클라이언트가 보낸 패킷 순서는 보장됩니다.
        ///     
        /// 주의 할점
        ///     이 매소드에서 다른 CPeer 객체를 참조하거나 공유자원에 접근할 때는 동기화 처리를 해줘야 합니다.
        ///     매소드가 리턴되면 buffer는 비워지며 다음 패킷을 담을 준비를 하기 떄문에 매소드를 리턴하기 전에 사용할 데이터를
        ///     모두 빼내야 합니다. buffer참조를 다른 객체에 넘긴 후 매소드가 리턴된 이후에사용하게 해서는 안된다.
        ///     이런 경우에는 참조대신 복사하여 처리해야 한다.
        ///     
        /// </summary>
        /// <param name="buffer"></param>
        void on_message(Const<byte[]> buffer);

        /// <summary>
        /// 원격 연결이 끊겼을 때 호출 됩니다.
        /// 이 매소드가 호출된 이후 부터는 데이터 전송이 불가능합니다.
        /// </summary>
        void on_removed();

        void send(CPacket msg);

        void disconnect();
        void process_user_operation(CPacket msg);
    }
}