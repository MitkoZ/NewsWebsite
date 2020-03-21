let WYSIWYGInstance; // if someone wants to reuse the previously created instance

function WYSIWYG(containerId, isDisabled) {
    // private functions
    function getQuillLength() {
        return quill.getLength(); // Retrieves the length of the editor contents. Note even when Quill is empty, there is still a blank line represented by ‘\n’, so getLength will return 1. (source: https://quilljs.com/docs/api/#getlength)
    }

    function trimQuillInput(hiddenInputId) {
        let contentInput = document.getElementById(hiddenInputId);
        if (getQuillLength() == 1) {
            contentInput.value = null;
        }
    }

    function enableHiddenFieldsValidation(formjQuery) {
        formjQuery.data("validator").settings.ignore = ""; // we need to enable hidden field validation for jquery validate (please note that we are also using jquery-validation-unobtrusive) (source: https://stackoverflow.com/a/11053251)
    }

    // public functions declaration to achieve OOP style rather than functional programming
    this.getQuill = function (isDisabled) {
        if (isDisabled) {
            let quill = new Quill("#" + containerId, {
                modules: {
                    toolbar: false
                },
                theme: "snow",
            });
            quill.disable();
            document.getElementById(containerId).style.border = "none";

            return quill;
        }
        else {
            return new Quill("#" + containerId, {
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

    this.populateInputWithWYSIWYGData = function (hiddenInputId) {
        let contentInput = document.getElementById(hiddenInputId);
        contentInput.value = JSON.stringify(quill.getContents());
    }

    this.populateContainerWithWYSIWYGData = function (data) {
        quill.setContents(data);
    }

    this.attachOnSubmitWYSIWYGHandler = function (formId, hiddenInputId) {
        let form = $("#" + formId);
        let that = this;
        form.submit(function () {
            // Populate hidden form input on submit
            that.populateInputWithWYSIWYGData(hiddenInputId);
            trimQuillInput(hiddenInputId);

            // validation (we need front-end validation, since QuillJs uses Deltas and the back-end can't get only the text data of it to accurately validate the length of the actual data)
            if (getQuillLength() < that.TEXT_EXCERPT_END_INDEX) {
                enableHiddenFieldsValidation(form);

                // we set the min length dynamically, so that the validation triggers
                $("#" + hiddenInputId).rules("add", {
                    minlength: that.TEXT_EXCERPT_END_INDEX
                });
            }

        });
    }

    this.getText = function (startIndex, endIndex) {
        return quill.getText(startIndex, endIndex);
    }

    this.setText = function setText(text) {
        quill.setText(text);
    }

    this.getShortExcerpt = function () {
        let textExcerpt = this.getText(0, this.TEXT_EXCERPT_END_INDEX);
        let continuationSymbol = "...";

        return textExcerpt + continuationSymbol;
    }

    //public properties
    this.TEXT_EXCERPT_END_INDEX = 100;

    // You should place your after instantiation logic here
    let quill = this.getQuill(isDisabled);
    WYSIWYGInstance = this;
}