var quill = new Quill('#editor-container', {
    modules: {
        toolbar: [
            [{ 'font': [] }],
            [{ 'indent': '-1' }, { 'indent': '+1' }],
            [{ 'direction': 'rtl' }],
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
            [{ 'color': [] }, { 'background': [] }],
            [{ 'align': [] }],
            ['bold', 'italic', 'underline', 'strike'],
            ['link', 'blockquote', 'image', 'video'],
            [{ list: 'ordered' }, { list: 'bullet' }],
            ['clean']
        ]
    },
    placeholder: 'Write your news...',
    theme: 'snow'
});
quill.setContents({ "ops": [{ "attributes": { "bold": true }, "insert": "gfdgfdgfdgfd" }, { "insert": "\n" }] });

var form = document.querySelector('form');
form.onsubmit = function () {
    // Populate hidden form on submit
    var about = document.querySelector('input[name=about]');
    about.value = JSON.stringify(quill.getContents());

    console.log("Submitted", $(form).serialize(), $(form).serializeArray());

    // No back end to actually submit to!
    alert('Open the console to see the submit data!')
    return false;
};
