function CheckInputAndReload() {
    var from = FromFilter;
    var to = ToFilter;

    if (FromFilter.getValue() === '' || ToFilter.getValue() === '') {
        //Ext.Msg.alert('Invalid Date Range', 'Select a valid Date Range');
        Ext.Msg.show(
        {
            icon: Ext.MessageBox.WARNING,
            msg: 'Select a valid Date Range.',
            buttons: Ext.Msg.OK,
            title: 'Invalid Date Range'
        });
    }
    else if (FilterTree.getCheckedNodes().length === 0) {
        Ext.Msg.show(
        {
            icon: Ext.MessageBox.WARNING,
            msg: 'Select at least one aspect in the treepane',
            buttons: Ext.Msg.OK,
            title: 'Invalid Filter Criteria'
        });
    }
    else {

        ObservationsGrid.getStore().load();
        
    }
    
}