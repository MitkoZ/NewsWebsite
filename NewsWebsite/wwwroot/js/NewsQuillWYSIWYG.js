let WYSIWYGInstance; // if someone wants to reuse the previously created instance

function WYSIWYG(containerId, isDisabled) {

    // private functions
    function getQuillLength() {
        return quill.getLength(); // Retrieves the length of the editor contents. Note even when Quill is empty, there is still a blank line represented by ‘\n’, so getLength will return 1. (source: https://quilljs.com/docs/api/#getlength)
    }

    function trimQuillInput(hiddenInputFilter) {
        let content = document.querySelector(hiddenInputFilter);
        if (getQuillLength() == 1) {
            content.value = null;
        }
    }

    // public functions declaration to achieve OOP style rather than functional programming
    this.getQuill = function (isDisabled) {
        if (isDisabled) {
            let quill = new Quill(containerId, {
                modules: {
                    toolbar: false
                },
                theme: "snow",
            });
            quill.disable();

            return quill;
        }
        else {
            return new Quill(containerId, {
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
        }

    };

    this.populateInputWithWYSIWYGData = function (hiddenInputFilter) {
        let content = document.querySelector(hiddenInputFilter);
        content.value = JSON.stringify(quill.getContents());
    }

    this.populateContainerWithWYSIWYGData = function (data) {
        quill.setContents(data);
    }

    this.attachOnSubmitWYSIWYGHandler = function (formId, hiddenInputFilter) {
        let form = document.getElementById(formId);
        let that = this;
        form.onsubmit = function () {
            // Populate hidden form input on submit
            that.populateInputWithWYSIWYGData(hiddenInputFilter);
            trimQuillInput(hiddenInputFilter);

            return true;
        };
    }


    // You should place your after instantiation logic here
    let quill = this.getQuill(isDisabled);
    WYSIWYGInstance = this;
}