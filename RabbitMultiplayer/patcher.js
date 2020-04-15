const uml = importNamespace('UndertaleModLib');
const umm = importNamespace('UndertaleModLib.Models');
const EType = umm.EventType;
const umd = importNamespace('UndertaleModLib.Decompiler');
const Decompile = umd.Decompiler.Decompile;
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
function decompileGML(codeO){
    return Decompile(codeO,new umd.DecompileContext(data,false));
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
    let locals = new umm.UndertaleCodeLocals();
    locals.Name = code.Name;
    data.CodeLocals.Add(locals);
    rewriteGMLPath(obj,code,path);
    return code;
}
function insertGML(codeO,line,code){
    let dec = decompileGML(codeO);
    let index = 0;
    for (let i=0;i<line;i++){
        index = dec.indexOf('\n',index)+1;
    }
    compileGML(codeO,dec.slice(0,index)+code+"\n"+dec.slice(index));
}
function replaceLineGML(codeO,line,code){
    let dec = decompileGML(codeO);
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
function insertScriptGML(scrname, line,path){
    insertGML(byName(data.Code,"Name","gml_Script_"+scrname),line,gmlTextScriptPath("scriptedits/"+path))
}
function replaceLinePathGML(obj, codeO, line, path){
    replaceLineGML(codeO,line,gmlTextObjectPath(obj,path))
}
function replaceGML(codeO,toReplace,replWith){
    compileGML(codeO,decompileGML(codeO).replace(toReplace,replWith))
}
function replacePathGML(obj, codeO, toReplace, path){
    replaceGML(codeO,toReplace,gmlTextObjectPath(obj,path));
}
function replaceScriptPathGML(obj, codeO, toReplace, path){
    replaceGML(codeO,toReplace,gmlTextObjectPath(obj,path));
}

/**
 * 
 * @param {string} name
 */
function createScriptNamed(name){
    let script = new umm.UndertaleScript();
    let path = name;
    if (name.includes("/"))name = name.substr(name.lastIndexOf("/")+1);
    script.Name = mstr(name);//data.Strings.MakeString wrapper (i don't have extension methods here)
    script.Code = new umm.UndertaleCode();
    script.Code.Name = mstr("gml_Script_"+name);
    data.Code.Add(script.Code);
    data.Scripts.Add(script);
    let locals = new umm.UndertaleCodeLocals();
    locals.Name = script.Code.Name;
    data.CodeLocals.Add(locals);
    compileGML(script.Code,gmlTextScriptPath(path));
    script.Code.UpdateAddresses();
}
function createScripts(names){
    for(let i=0;i<names.length;i++)createScriptNamed(names[i]);
}

function multiplayer(){
    let net = new umm.UndertaleGameObject();
    let faker = new umm.UndertaleGameObject();
    createScripts([
        "menu_connect",
        "send_message",
        "setup_buffer",
        "writes/wop_connect",
        "writes/wop_disconnect",
        "writes/wop_movement",
        "writes/wop_playerconnect",
        "reads/rop_connect",
        "reads/rop_disconnect",
        "reads/rop_movement",
        "reads/rop_playerconnect",
    ]);
    //use brackets to separate scope to allow for less confusing variable reuse
    {
        faker.Name = mstr("obj_faker");
        faker.Persistent = true;
        faker.Sprite = byName(data.Sprites,"Name","spr_playeridle");
        data.GameObjects.Add(faker);
    }
    {
        net.Name = mstr("obj_net");
        net.Persistent = true;
        data.GameObjects.Add(net);
        createEvent(net, 0, 0, "create");
        createEvent(net, 3, 2, "end_step");
        createEvent(net, 7, 68,"net_async");
        createEvent(net, 7, 3, "game_end");
    }
    insertScriptGML("savesettings",17, "savesettings");
    insertScriptGML("loadsettings", 9, "loadsettings");
    {
        let go = new RoomGameObject();
        go.InstanceID = 108990;
        go.GMS2_2_2 = true;
        go.ObjectDefinition = net;
        let initrm = byName(data.Rooms,"Name","rm_init");
        initrm.GameObjects.Add(go);
        byName(initrm.Layers,"LayerName","Instances").InstancesData.Instances.Add(go);
    }
    //createEvent(byName(data.GameObjects,"Name","obj_camera"),8,72,"pre_draw");
    replaceGML(getEvent(byName(data.GameObjects,"Name","obj_menus"),0,0),
        "ds_pmenu_main = create_menu_page(",
        "ds_pmenu_main = create_menu_page([\"DEBUG MENU\",1,10],");
    {//main menu 
        let mainmenu = byName(data.GameObjects,"Name","obj_mainmenus");
        let create = getEvent(mainmenu,0,0);
        insertLineGML(mainmenu,create,16,"createPage1");
        replaceGML(create,"[\"ERASE FILE\", 1, 10]", "[\"ERASE FILE\", 1, 10], [\"COPY FILE\", 1, 9]");
        replaceLineGML(create,16,`ds_menu_main = create_menu_page(["START GAME", 1, 8], ["SETTINGS", 1, 1],`+
            `["MULTI-PLAYER", 1, 19], ["EXIT GAME", 0, 185])`);
        insertGML(create,44,"global.menu_pages[array_length_1d(global.menu_pages)] = ds_menu_net;");
        let step = getEvent(mainmenu,3,0);
        replaceGML(step,"var ochange","ochange");
        insertLineGML(mainmenu, step,260,"stepPage2");
        /*insertGML(step, 298, "switch (global.menu_option[global.page]){" +
            "case 0: global.CurrentFile = \"savedfile.sav\"; break;" +
            "default: global.CurrentFile = \"savedfile\"+" +
            "string(global.menu_option[global.page]+1)+\".sav\"; break;}");*/
        insertLineGML(mainmenu, step, 47, "stepPage1");
        insertGML(step, 0, "heylois = 0;");
        let draw = getEvent(mainmenu, 8,0);
        insertLineGML(mainmenu, draw, 115, "drawPage1");
        insertLineGML(mainmenu,draw, 62, "drawPage2");
    }
}
function dump() {
    System.IO.Directory.CreateDirectory("gml/dumps");
    for(let i=0;i<data.Code.Count;i++){
        let code = data.Code[i];
        let str = decompileGML(code);
        System.IO.File.WriteAllText("gml/dumps/"+code.Name.Content+".gml", str);
        System.IO.File.WriteAllText("gml/dumps/"+code.Name.Content+".gml.md5", emmdeefive(str));
    }
}
function pdev(){//patchdev 
    NetDiff.DiffUtil
}
function main(){//patch 
    log("If the patcher fails and data.win is gone, replace it with data.win.bak");
    log("If data.win.bak is gone, verify integrity of the game on Steam, or redownload the game if on itch.io");
    log("Steam: https://support.steampowered.com/kb_article.php?ref=2037-QEUH-3335");
    log("Itch: https://studionevermore.itch.io/oh-jeez-oh-no-my-rabbits-are-gone");
    log("=======================");
    log("Now dumping the game to ./gml/dumps/*");
    dump();
    log("Dumped the game's files to ./gml/dumps/*");
    log("Patching multiplayer...");
    multiplayer();
    log("Done patching multiplayer.");
}