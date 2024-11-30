(function($) {
  'use strict';
    $('#deleteModal').on('show.bs.modal', function (event) {
    console.log('flkjafl');
    var button = $(event.relatedTarget) // Button that triggered the modal
    var urlToGo = button.data('urlToGo') // Extract info from data-* attributes
    var deleteType = button.data('deleteType')
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this)
    modal.find('.modal-title').text('Are You Sure You Want to Delete This' + deleteType)
    modal.find('.modal-body input').val(urlToGo)
  })
})(jQuery);