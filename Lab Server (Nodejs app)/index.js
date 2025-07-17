const express = require('express');
const http = require('http');
const fs = require('fs');
const WebSocket = require('ws');
const { BuildRoomDataList } = require('./logger');
const LabRoom = require("./lab-room").LabRoom;

const PORT = 55050;
const SERVER_NAME = "localhost-1";
const LAB_ROOMS = new Map();
let LAST_LAB_ROOM;

const json_style = fs.readFileSync("./json_style.css")+""

const app = express();
const server = http.createServer(app);
const wss = new WebSocket.Server({ server });

wss.on('connection', (client) => {
    console.log("client connected: ")
    client.actions = {
        create({name, uid, dp}){
            client.uid = uid;
            let roomID = "room_"+Date.now();
            let newRoom = new LabRoom(uid);
            newRoom.roomID = roomID;
            LAB_ROOMS.set(roomID, newRoom);
            newRoom.Join(client, {name, uid, dp});
            LAST_LAB_ROOM = newRoom;
        },
        join({name, uid, dp, roomID}){
            if(!LAB_ROOMS.has(roomID)){
                console.log("ROOM DOES NOT EXIST");
                return;
            }
            let oldRoom = LAB_ROOMS.get(roomID);
            oldRoom.Join(client, {name, uid, dp});
        }
    }
    client.on('message', (message) => {
        //console.log(message + "")
        let {action, data, extra} = JSON.parse(message);
        try{
            let tmp = JSON.parse(data);
            if(tmp) data = tmp;
        }
        catch(e){}
        if(client.actions[action]) client.actions[action](data, extra);
    });
    client.on("close", ()=>{
        if(client.room){
            client.room.Leave(client);
        }
    });
});

app.get("/name", (req, res)=>{
    res.end(SERVER_NAME);
})

app.get("/roomlist", async (req, res)=>{
    let list = [];
    LabRoom.ROOMS.forEach(room=>{
        const hostUID = room.hostUID;
        list.push(JSON.stringify({
            roomID : room.roomID,
            hostName : room.CLIENTS[hostUID].name,
            dp : room.CLIENTS[hostUID].dp,
            size : room.GetClientCount() + "/" + room.maxMember
        }))
    })
    if(LabRoom.LATENCY > 0) await new Promise(r=>setTimeout(r, LabRoom.LATENCY))
    res.end(list.join("*"))
})
app.get("/debug_1", (req, res)=>{
    console.log(LAST_LAB_ROOM)
    res.end(""+Date.now())
})
app.get("/gs", (req, res)=>{
    console.log("GS")
    res.end(process.uptime().toString())
})
app.get("/log", (req, res)=>{
    console.log("LOG")
    const data = BuildRoomDataList();
    res.send(`<style>${json_style}</style>`);
    if(data.trim().length == 0) res.end("<h1>No Room</h1>");
    else res.send(data);
    res.end();
})

// Start the server on port 8080
server.listen(PORT, () => {
  console.log('Server is listening on http://localhost:'+PORT);
});
