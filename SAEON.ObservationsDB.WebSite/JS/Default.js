function NewTab(node) {
    hostName = window.location.protocol + "//" + window.location.host + "/";

    var tab = new Ext.Panel({
        id: node.id,
        title: node.text,
        hideMode: "offsets",
        autoLoad: {
            showMask: true,
            scripts: true,
            mode: "iframe",
            url: hostName + node.attributes.href,
            nocache:true 
        },
        maximizable: false,
        constrain: false,
        closeAction: "hide",
        closable: true,
        iconCls: node.attributes.iconCls
    });
    
    PanelC.add(tab)

    PanelC.setActiveTab(tab);
}