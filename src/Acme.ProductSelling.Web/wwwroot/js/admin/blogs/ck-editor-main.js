const apiUrl = "/api/files/upload-image";

const {
    ClassicEditor,
    Alignment,
    Autoformat,
    AutoImage,
    AutoLink,
    Autosave,
    BlockQuote,
    Bold,
    Bookmark,
    CloudServices,
    Code,
    CodeBlock,
    Emoji,
    Essentials,
    FindAndReplace,
    FontBackgroundColor,
    FontColor,
    FontFamily,
    FontSize,
    FullPage,
    Fullscreen,
    GeneralHtmlSupport,
    Heading,
    Highlight,
    HorizontalLine,
    HtmlComment,
    HtmlEmbed,
    ImageBlock,
    ImageCaption,
    ImageInline,
    ImageInsert,
    ImageInsertViaUrl,
    ImageResize,
    ImageStyle,
    ImageTextAlternative,
    ImageToolbar,
    ImageUpload,
    Indent,
    IndentBlock,
    Italic,
    Link,
    LinkImage,
    List,
    ListProperties,
    MediaEmbed,
    Mention,
    PageBreak,
    Paragraph,
    PasteFromOffice,
    PlainTableOutput,
    RemoveFormat,
    ShowBlocks,
    SimpleUploadAdapter,
    SourceEditing,
    SpecialCharacters,
    SpecialCharactersArrows,
    SpecialCharactersCurrency,
    SpecialCharactersEssentials,
    SpecialCharactersLatin,
    SpecialCharactersMathematical,
    SpecialCharactersText,
    Strikethrough,
    Style,
    Subscript,
    Superscript,
    Table,
    TableCaption,
    TableCellProperties,
    TableColumnResize,
    TableLayout,
    TableProperties,
    TableToolbar,
    TextPartLanguage,
    TextTransformation,
    TodoList,
    Underline,
    WordCount
} = window.CKEDITOR;

const LICENSE_KEY = 'eyJhbGciOiJFUzI1NiJ9.eyJleHAiOjE3ODgxMzQzOTksImp0aSI6IjRkMjYzZjhjLTdmZGMtNGM5Yy1iOTdhLTAxY2NlMjJmY2RlZSIsImxpY2Vuc2VkSG9zdHMiOlsiMTI3LjAuMC4xIiwibG9jYWxob3N0IiwiMTkyLjE2OC4qLioiLCIxMC4qLiouKiIsIjE3Mi4qLiouKiIsIioudGVzdCIsIioubG9jYWxob3N0IiwiKi5sb2NhbCJdLCJ1c2FnZUVuZHBvaW50IjoiaHR0cHM6Ly9wcm94eS1ldmVudC5ja2VkaXRvci5jb20iLCJkaXN0cmlidXRpb25DaGFubmVsIjpbImNsb3VkIiwiZHJ1cGFsIl0sImxpY2Vuc2VUeXBlIjoiZGV2ZWxvcG1lbnQiLCJmZWF0dXJlcyI6WyJEUlVQIiwiRTJQIiwiRTJXIl0sInZjIjoiMjg5NjkyYTQifQ.WRH44VEgYYciw0a-A-FOw0VkNc07_s_ClqiD_D8enotxUPM9rZzyZJYZ-cBIaHU5ppL-wds1cyyDyPwhTGvjUA';

const editorConfig = {
    toolbar: {
        items: [
            'undo', 'redo', '|',
            'heading', '|',
            'bold', 'italic', 'underline', 'strikethrough', 'removeFormat', '|',
            'bulletedList', 'numberedList', '|',
            'link', 'insertImage', 'blockQuote', '|',
            'alignment'
        ],
        shouldNotGroupWhenFull: true
    },
    plugins: [
        Alignment, Autoformat, AutoImage, AutoLink, Autosave,
        BlockQuote, Bold, Bookmark, CloudServices, Code, CodeBlock,
        Emoji, Essentials, FindAndReplace, FontBackgroundColor, FontColor,
        FontFamily, FontSize, FullPage, Fullscreen, GeneralHtmlSupport,
        Heading, Highlight, HorizontalLine, HtmlComment, HtmlEmbed,
        ImageBlock, ImageCaption, ImageInline, ImageInsert, ImageInsertViaUrl,
        ImageResize, ImageStyle, ImageTextAlternative, ImageToolbar, ImageUpload,
        Indent, IndentBlock, Italic, Link, LinkImage, List, ListProperties,
        MediaEmbed, Mention, PageBreak, Paragraph, PasteFromOffice,
        PlainTableOutput, RemoveFormat, ShowBlocks, SimpleUploadAdapter,
        SourceEditing, SpecialCharacters, SpecialCharactersArrows,
        SpecialCharactersCurrency, SpecialCharactersEssentials, SpecialCharactersLatin,
        SpecialCharactersMathematical, SpecialCharactersText, Strikethrough,
        Style, Subscript, Superscript, Table, TableCaption, TableCellProperties,
        TableColumnResize, TableLayout, TableProperties, TableToolbar,
        TextPartLanguage, TextTransformation, TodoList, Underline, WordCount
    ],
    fontFamily: { supportAllValues: true },
    fontSize: {
        options: [10, 12, 14, 'default', 18, 20, 22],
        supportAllValues: true
    },
    fullscreen: {
        onEnterCallback: container =>
            container.classList.add(
                'editor-container',
                'editor-container_classic-editor',
                'editor-container_include-style',
                'editor-container_include-word-count',
                'editor-container_include-fullscreen',
                'main-container'
            )
    },
    heading: {
        options: [
            { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
            { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
            { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' },
            { model: 'heading3', view: 'h3', title: 'Heading 3', class: 'ck-heading_heading3' },
            { model: 'heading4', view: 'h4', title: 'Heading 4', class: 'ck-heading_heading4' },
            { model: 'heading5', view: 'h5', title: 'Heading 5', class: 'ck-heading_heading5' },
            { model: 'heading6', view: 'h6', title: 'Heading 6', class: 'ck-heading_heading6' }
        ]
    },
    htmlSupport: {
        allow: [{ name: /^.*$/, styles: true, attributes: true, classes: true }]
    },
    image: {
        toolbar: [
            'toggleImageCaption', 'imageTextAlternative', '|',
            'imageStyle:inline', 'imageStyle:wrapText', 'imageStyle:breakText', '|',
            'resizeImage'
        ]
    },
    licenseKey: LICENSE_KEY,
    link: {
        addTargetToExternalLinks: true,
        defaultProtocol: 'https://',
        decorators: {
            toggleDownloadable: {
                mode: 'manual',
                label: 'Downloadable',
                attributes: { download: 'file' }
            }
        }
    },
    list: {
        properties: { styles: true, startIndex: true, reversed: true }
    },
    mention: {
        feeds: [{ marker: '@', feed: [] }]
    },
    menuBar: { isVisible: true },
    placeholder: 'Type or paste your product description here!',
    style: {
        definitions: [
            { name: 'Article category', element: 'h3', classes: ['category'] },
            { name: 'Title', element: 'h2', classes: ['document-title'] },
            { name: 'Subtitle', element: 'h3', classes: ['document-subtitle'] },
            { name: 'Info box', element: 'p', classes: ['info-box'] },
            { name: 'CTA Link Primary', element: 'a', classes: ['button', 'button--green'] },
            { name: 'CTA Link Secondary', element: 'a', classes: ['button', 'button--black'] },
            { name: 'Marker', element: 'span', classes: ['marker'] },
            { name: 'Spoiler', element: 'span', classes: ['spoiler'] }
        ]
    },
    table: {
        contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells', 'tableProperties', 'tableCellProperties']
    },
    simpleUpload: {
        uploadUrl: apiUrl,
        withCredentials: true,
        headers: {
            'X-CSRF-TOKEN': document.querySelector('meta[name="csrf-token"]')?.getAttribute('content') || '',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        }
    }
};

let ckProductDescriptionEditor = null;

document.addEventListener('DOMContentLoaded', function () {
    const productDescEditorEl = document.querySelector('#productDescriptionEditor');
    if (productDescEditorEl) {
        ClassicEditor
            .create(productDescEditorEl, editorConfig)
            .then(editor => {
                ckProductDescriptionEditor = editor;
            })
            .catch(error => {
                console.error('Error initializing CKEditor (product description):', error);
            });
    }

    const productForm = document.querySelector('#createProductForm');
    if (productForm) {
        productForm.addEventListener('submit', function () {
            if (ckProductDescriptionEditor) {
                document.querySelector('#productDescriptionEditor').value =
                    ckProductDescriptionEditor.getData();
            }
        });
    }
});