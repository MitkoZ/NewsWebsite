let quill = new Quill('#editor-container', {
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

let form = document.getElementById("news-form");
form.onsubmit = function () {
    // Populate hidden form input on submit
    populateHiddenInputWithQuillData();
    trimQuillInput();

    console.log("Submitted", $(form).serialize(), $(form).serializeArray());
    return true;
};

function populateHiddenInputWithQuillData() {
    let content = document.querySelector('input[name=Content]');
    content.value = JSON.stringify(quill.getContents());
}

function populateVisibleContainerWithQuillData(data) {
    quill.setContents(data);
}

function getQuillLength() {
    return quill.getLength(); // Retrieves the length of the editor contents. Note even when Quill is empty, there is still a blank line represented by ‘\n’, so getLength will return 1. (source: https://quilljs.com/docs/api/#getlength)
}

function trimQuillInput() {
    let content = document.querySelector('input[name=Content]');
    if (getQuillLength() == 1) {
        content.value = null;
    }
}