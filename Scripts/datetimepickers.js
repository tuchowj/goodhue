﻿$('#startDate').bootstrapMaterialDatePicker({ format: 'MM/DD/YYYY hh:mm A', shortTime: true, currentDate: moment({hour:8})});
$('#endDate').bootstrapMaterialDatePicker({ format: 'MM/DD/YYYY hh:mm A', shortTime: true, currentDate: moment({ hour: 17 })});
$('#date').bootstrapMaterialDatePicker({ format: 'MM/DD/YYYY', time: false })
$('.date').bootstrapMaterialDatePicker({ format: 'MM/DD/YYYY', time: false, clearButton: true })