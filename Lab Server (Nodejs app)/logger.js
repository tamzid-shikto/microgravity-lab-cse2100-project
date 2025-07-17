const { LabRoom } = require("./lab-room");
let ppj;
(async ()=>{
    const t = await import("pretty-print-json")
    ppj = t.prettyPrintJson
})()

exports.BuildRoomDataList = ()=>{
    const LIST = [];
    LabRoom.ROOMS.forEach(room=>{
        const data = {
            _roomID: room.roomID,
            _clientCount: [Object.keys(room.CLIENTS).length, room.maxMember],
            _simulationPlaying: room.simulationPlaying,
            _hostUID: room.hostUID,
            clients: [],
        }
        Object.entries(room.CLIENTS).forEach(([k,v])=>{
            const tc = {}
            tc.name = v.name;
            tc.uid = v.uid;
            data.clients.push(tc);
        })
        LIST.push(ppj.toHtml((data)));
        
    });

    return LIST.join("<hr>");
}