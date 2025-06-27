mergeInto(LibraryManager.library, {
  UploadFile: function () {
    var input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.onchange = e => {
      var file = e.target.files[0];
      var reader = new FileReader();
      reader.onload = function (event) {
        var base64Data = event.target.result;
        SendMessage('InferenceService', 'ReceiveImageData', base64Data);
      };
      reader.readAsDataURL(file);
    };
    input.click();
  }
});
