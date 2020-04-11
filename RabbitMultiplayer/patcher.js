const uml = importNamespace('UndertaleModLib');
const umm = importNamespace('UndertaleModLib.Models');
const EType = umm.EventType;
const umd = importNamespace('UndertaleModLib.Decompiler');
const types = [
    "Create",
    "Destroy",
    "Alarm",
    "Step",
    "Collision",
    "Keyboard",
    "Mouse",
    "Other",
    "Draw",
    "KeyPress",
    "KeyRelease",
    "Gesture",
    "Asynchronous",
    "Unknown13",
    "PreCreate",
];
function mstr(string){
    let str = new umm.UndertaleString(string);
    data.Strings.Add(str);
    return str;
}
function fstr(string) {
    for(let i=0;i<data.Strings.Count;i++){
        if (data.Strings[i].Content === string) return data.Strings[i];
    }
    return null;
}
function byName(list,nN,name){
    for(let i=0;i<list.Count;i++){
        if (list[i][nN].Content === name)return list[i];
    }
    return null;
}
function gmlTextScriptPath(path){
    return System.IO.File.ReadAllText("gml/scripts/"+path+".gml")
}
function gmlTextObjectPath(obj, path){
    return System.IO.File.ReadAllText("gml/"+obj.Name.Content+"/"+path+".gml")
}
function compileGML(codeO,gml){
    codeO.ReplaceGML(gml,data)
}
function rewriteGMLPath(obj,codeO,path){
    compileGML(codeO,gmlTextObjectPath(obj,path))
}
function getEvent(obj,type,subtype){
    let name = "gml_Object_"+obj.Name.Content+"_"+types[type]+"_"+subtype;
    let e = byName(data.Code,"Name",name);
    if (e == null){
        e = new umm.UndertaleCode();
        e.Name = mstr(name);
        data.Code.Add(e);
        let ea = new EventAction();
        ea.CodeId = e;
        let evt = new UEvent();
        evt.EventSubtype = subtype;
        evt.Actions.Add(ea);
        obj.Events[type].Add(evt);
    }
    return e;
}
function createEvent(obj,type,subtype,path){
    let code = getEvent(obj,type,subtype);
    rewriteGMLPath(obj,code,path);
    return code;
}
function insertGML(codeO,line,code){
    let dec = umd.Decompiler.Decompile(codeO,new umd.DecompileContext(data,false));
    let index = 0;
    for (let i=0;i<line;i++){
        index = dec.indexOf('\n',index)+1;
    }
    compileGML(codeO,dec.slice(0,index)+code+"\n"+dec.slice(index));
}
function replaceLineGML(codeO,line,code){
    let dec = umd.Decompiler.Decompile(codeO,new umd.DecompileContext(data,false));
    let firstIndex = 0;
    let secondIndex = 0;
    for (let i=0;i!==line;i++){
        firstIndex = secondIndex;
        secondIndex = dec.indexOf('\n',secondIndex)+1;
    }
    compileGML(codeO,dec.slice(0,firstIndex)+code+"\n"+dec.slice(secondIndex));
}
function insertLineGML(obj,codeO,line,path){
    insertGML(codeO,line,gmlTextObjectPath(obj,path))
}
function replaceGML(codeO,toReplace,replWith){
    compileGML(codeO,umd.Decompiler.Decompile(codeO,new umd.DecompileContext(data,false)).replace(toReplace,replWith))
}
function replacePathGML(obj, codeO, toReplace, path){
    replaceGML(codeO,toReplace,gmlTextObjectPath(obj,path));
}
function createScriptNamed(name){
    let script = new umm.UndertaleScript();
    script.Name = mstr(name);
    script.Code = new umm.UndertaleCode();
    script.Code.Name = mstr("gml_Script_"+name);
    data.Code.Add(script.Code);
    data.Scripts.Add(script);
    compileGML(script.Code,gmlTextScriptPath(name));
    script.Code.UpdateAddresses();
}
function createScripts(names){
    for(let i=0;i<names.length;i++)createScriptNamed(names[i]);
}

function multiplayer(){
    let net = new umm.UndertaleGameObject();
    createScripts([
        "menu_connect",
        "send_message",
        "setup_buffer",
        "writes/wop_connect",
        "writes/wop_disconnect",
        "writes/wop_movement",
        "reads/rop_connect",
        "reads/rop_disconnect",
        "reads/rop_movement"
    ]);
    //use brackets to separate scope to allow for less confusing variable reuse
    {
        net.Name = mstr("obj_net");
        net.Persistent = true;
        data.GameObjects.Add(net);
        createEvent(net, 0, 0, "create");
        createEvent(net, 3, 2, "end_step");
        createEvent(net, 7, 68, "net_async")
    }
    {
        let go = new RoomGameObject();
        go.InstanceID = 108990;
        go.GMS2_2_2 = true;
        go.ObjectDefinition = net;
        let initrm = byName(data.Rooms,"Name","rm_init");
        initrm.GameObjects.Add(go);
        byName(initrm.Layers,"LayerName","Instances").InstancesData.Instances.Add(go);
    }
    {//main menu 
        let mainmenu = byName(data.GameObjects,"Name","obj_mainmenus");
        let create = getEvent(mainmenu,0,0);
        insertLineGML(mainmenu,create,16,"createPage1");
        replaceLineGML(create,16,`ds_menu_main = create_menu_page(["START GAME", 1, 8], ["SETTINGS", 1, 1],`+
        `["MULTI-PLAYER", 1, 19], ["EXIT GAME", 0, 185])`);
        insertGML(create,50,"global.menu_pages[array_length_1d(global.menu_pages)] = ds_menu_net;");
        let step = getEvent(mainmenu,3,0);
        replaceLineGML(step,202,`case 9:cursorpos = string_length(variable_instance_get(ds_grid_get(ds_grid, 2, yy), ds_grid_get(ds_grid, 3, yy)));\ncase 5:`);
        insertLineGML(mainmenu, step, 47, "steppage1");
        let draw = getEvent(mainmenu, 8,0);
        insertLineGML(mainmenu, draw, 115, "drawPage");
    }
}
function main(){
    log("If the patcher fails and data.win is gone, replace it with data.win.bak");
    log("If data.win.bak is gone, verify integrity of the game on Steam, or redownload the game if on itch.io");
    log("Steam: https://support.steampowered.com/kb_article.php?ref=2037-QEUH-3335");
    log("Itch: https://studionevermore.itch.io/oh-jeez-oh-no-my-rabbits-are-gone");
    log("=======================");
    log("Patching multiplayer...");
    multiplayer();
    log("Done patching multiplayer.");
}