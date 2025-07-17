class LabRoom{
    deleteRoomCB;
    CLIENTS = {};
    ENV_SENT = {};
    roomID = "";
    hostUID = "";
    gravity = 0;
    maxMember = 10;
    activeExperimenterUID = "";
    simulationPlaying = "True";
    tick = 0;
    envCache;

    static ROOMS = []
    static LATENCY = 0;

    constructor(hostUID){
        console.log("new room created")
        LabRoom.ROOMS.push(this);

        //this.hostUID = hostUID;
        this.activeExperimenterUID = "";
    }

    MakeHost(client){
        this.hostUID = client.uid;
        console.log("Made Host: ", client.uid, client.name)
    }

    GetClientCount(){
        return Object.keys(this.CLIENTS).length;
    }

    Join(client, {dp, name, uid}){
        console.log("New Join", {roomID:this.roomID, name, uid})
        this.CLIENTS[uid] = client;
        client.dp = dp;
        client.name = name;
        client.uid = uid;
        client.role = 1;
        client.room = this;

        //console.log(Object.keys(this.CLIENTS).length)

        if(this.GetClientCount() == 1) this.MakeHost(client);

        this.SendToClient(client, "SC_join_done", "")
        this.$_BroadCastRoomData();
        this.SendSimulationState(client);
        this.BroadCastToAllExcept(client, "SC_request_env_cache", "", uid);
        this.$_BroadCastChatEvent("join", name);

        client.actions.simulation_state = (data)=>{
            console.log("test", data);
        }
        client.actions.CS_spawn_object = (data)=>{
            this.BroadCastToAll("SC_spawn_object", data);
        }
        client.actions.CS_modify_object = (data)=>{
            console.log("CS_modify_object", data);
            this.BroadCastToAllExcept(client, "SC_modify_object", data);
        }
        client.actions.CS_gravity = ({x, y})=>{
            this.gravity = y;
            this.$_BroadCastRoomData();
        }
        client.actions.CS_chat_message = (text)=>{
            text = btoa(JSON.stringify(text));
            this.$_BroadCastChatEvent("message", text, client);
        }
        client.actions.CS_play_pause = ({state, tick})=>{
            if(state == "False" && this.activeExperimenterUID == ""){
                this.activeExperimenterUID = client.uid;
                this.simulationPlaying = state;
                this.tick = tick;
                this.$_BroadCastSendSimulationState();
                this.$_BroadCastChatEvent("pause", client.name);
            }
            else if(state == "True" && this.activeExperimenterUID == client.uid){
                this.activeExperimenterUID = "";
                this.simulationPlaying = state;
                this.tick = tick;
                this.$_BroadCastSendSimulationState();
                this.$_BroadCastChatEvent("play", client.name);
            }
            else {
                this.SendToClient(client, "SC_alert", "Someone else is doing experiment");
            }
        };
        client.actions.CS_reply_env_cache = (data, extra)=>{
            if(extra == ""){
                this.envCache = data;
                return;
            }
            if(this.ENV_SENT[extra]) return;
            this.ENV_SENT[extra] = true;
            let targetClient = this.CLIENTS[extra];
            if(targetClient) {
                console.log("try send cache to: ", extra);
                this.SendToClient(targetClient, "SC_reply_env_cache", data);
            }
        }
        client.actions.CS_spawn_ghost = (data)=>{
            this.BroadCastToAllExcept(client, "SC_spawn_ghost", data);
        }
        client.actions.CS_move_ghost = (data)=>{
            this.BroadCastToAllExcept(client, "SC_move_ghost", data);
            console.count("ghost move")
        }
    }

    Leave(client){
        console.log("Leave", client.uid)
        delete this.CLIENTS[client.uid]
        this.$_BroadCastRoomData();
        this.$_BroadCastChatEvent("leave", client.name);
        if(Object.keys(this.CLIENTS).length == 0) {
            console.log("Delete room", this.roomID)
            LabRoom.ROOMS.splice(LabRoom.ROOMS.indexOf(this));
        }
    }

    $_BroadCastChatEvent(eventName, data, sourceClient){
        [...Object.values(this.CLIENTS)].forEach(client=>{
            if(sourceClient && client == sourceClient) return;
            this.SendToClient(client, "SC_chat_event", {
                eventName, data
            });
        })
        if(sourceClient) {
            this.SendToClient(sourceClient, "SC_chat_event", {
                eventName:"self", data
            });
        }
    }

    $_BroadCastRoomData(){
        [...Object.values(this.CLIENTS)].forEach(client=>{
            this.SendToClient(client, "SC_room_data", this.BuildRoomData());
        })
    }
    $_BroadCastSendSimulationState(){
        [...Object.values(this.CLIENTS)].forEach(client=>{
            this.SendSimulationState(client);
        })
    }
    SendSimulationState(client){
        this.SendToClient(client, "SC_sim_state_data", this.BuildSimulationStateData());
    }

    BroadCastToAllExcept(exClient, action, data, extra){
        [...Object.values(this.CLIENTS)].forEach(client=>{
            if(client != exClient) this.SendToClient(client, action, data, extra);
        })
    }
    BroadCastToAll(action, data, extra=""){
        this.BroadCastToAllExcept(null, action, data, extra);
    }

    async SendToClient(client, action, data, extra=""){
        try{
            data = JSON.stringify(data);
        }catch(e){}
        const message = JSON.stringify({action, data, extra});
        if(LabRoom.LATENCY > 0) await new Promise(r=>setTimeout(r, LabRoom.LATENCY))
        client.send(message);
    }
    BuildRoomData(){
        return{
            hostUID: this.hostUID,
            gravity: this.gravity,
            maxMember: this.maxMember,
            clients: (_=>{
                let tmp = [];
                [...Object.values(this.CLIENTS)].forEach(client=>{
                    tmp.push({
                        name : client.name,
                        uid: client.uid,
                        role: client.role,
                        dp : client.dp,
                    });
                })
                return tmp;
            })()
        }
    }
    BuildSimulationStateData(){
        return {            
            simulationPlaying: this.simulationPlaying,
            tick: this.tick,
            activeExperimenterUID: this.activeExperimenterUID,
        }
    }
}

exports.LabRoom = LabRoom;