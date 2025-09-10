// CKEditor 5 Configuration and Initialization
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
    Markdown,
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
    Title,
    TodoList,
    Underline,
    WordCount
} = window.CKEDITOR;

const LICENSE_KEY = 'eyJhbGciOiJFUzI1NiJ9.eyJleHAiOjE3NTc4MDc5OTksImp0aSI6IjVhY2FjNmViLTcyZjctNGIzMC1hOTlmLWQwYmZjNGZkMTM5OSIsInVzYWdlRW5kcG9pbnQiOiJodHRwczovL3Byb3h5LWV2ZW50LmNrZWRpdG9yLmNvbSIsImRpc3RyaWJ1dGlvbkNoYW5uZWwiOlsiY2xvdWQiLCJkcnVwYWwiLCJzaCJdLCJ3aGl0ZUxhYmVsIjp0cnVlLCJsaWNlbnNlVHlwZSI6InRyaWFsIiwiZmVhdHVyZXMiOlsiKiJdLCJ2YyI6ImY2NzIyZTNlIn0.fNGID-WwTd1wZ9jvWBucm45ysky4grBjUG3BZY_uPxL6Ub3waACMI1CPsyjb82Fp0i5h6B8Bw9lsz63APU9aMA';

const editorConfig = {
    toolbar: {
        items: [
            'undo',
            'redo',
            '|',
            'sourceEditing',
            'showBlocks',
            'findAndReplace',
            'textPartLanguage',
            'fullscreen',
            '|',
            'heading',
            'style',
            '|',
            'fontSize',
            'fontFamily',
            'fontColor',
            'fontBackgroundColor',
            '|',
            'bold',
            'italic',
            'underline',
            'strikethrough',
            'subscript',
            'superscript',
            'code',
            'removeFormat',
            '|',
            'emoji',
            'specialCharacters',
            'horizontalLine',
            'pageBreak',
            'link',
            'bookmark',
            'insertImage',
            'insertImageViaUrl',
            'mediaEmbed',
            'insertTable',
            'insertTableLayout',
            'highlight',
            'blockQuote',
            'codeBlock',
            'htmlEmbed',
            '|',
            'alignment',
            '|',
            'bulletedList',
            'numberedList',
            'todoList',
            'outdent',
            'indent'
        ],
        shouldNotGroupWhenFull: true
    },
    plugins: [
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
        //Markdown,
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
        Title,
        TodoList,
        Underline,
        WordCount
    ],
    fontFamily: {
        supportAllValues: true
    },
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
            {
                model: 'paragraph',
                title: 'Paragraph',
                class: 'ck-heading_paragraph'
            },
            {
                model: 'heading1',
                view: 'h1',
                title: 'Heading 1',
                class: 'ck-heading_heading1'
            },
            {
                model: 'heading2',
                view: 'h2',
                title: 'Heading 2',
                class: 'ck-heading_heading2'
            },
            {
                model: 'heading3',
                view: 'h3',
                title: 'Heading 3',
                class: 'ck-heading_heading3'
            },
            {
                model: 'heading4',
                view: 'h4',
                title: 'Heading 4',
                class: 'ck-heading_heading4'
            },
            {
                model: 'heading5',
                view: 'h5',
                title: 'Heading 5',
                class: 'ck-heading_heading5'
            },
            {
                model: 'heading6',
                view: 'h6',
                title: 'Heading 6',
                class: 'ck-heading_heading6'
            }
        ]
    },
    htmlSupport: {
        allow: [
            {
                name: /^.*$/,
                styles: true,
                attributes: true,
                classes: true
            }
        ]
    },
    image: {
        toolbar: [
            'toggleImageCaption',
            'imageTextAlternative',
            '|',
            'imageStyle:inline',
            'imageStyle:wrapText',
            'imageStyle:breakText',
            '|',
            'resizeImage'
        ]
    },
    // Remove the initialData since we're using it with a form field
    licenseKey: LICENSE_KEY,
    link: {
        addTargetToExternalLinks: true,
        defaultProtocol: 'https://',
        decorators: {
            toggleDownloadable: {
                mode: 'manual',
                label: 'Downloadable',
                attributes: {
                    download: 'file'
                }
            }
        }
    },
    list: {
        properties: {
            styles: true,
            startIndex: true,
            reversed: true
        }
    },
    mention: {
        feeds: [
            {
                marker: '@',
                feed: [
                    /* See: https://ckeditor.com/docs/ckeditor5/latest/features/mentions.html */
                ]
            }
        ]
    },
    menuBar: {
        isVisible: true
    },
    placeholder: 'Type or paste your blog content here!',
    style: {
        definitions: [
            {
                name: 'Article category',
                element: 'h3',
                classes: ['category']
            },
            {
                name: 'Title',
                element: 'h2',
                classes: ['document-title']
            },
            {
                name: 'Subtitle',
                element: 'h3',
                classes: ['document-subtitle']
            },
            {
                name: 'Info box',
                element: 'p',
                classes: ['info-box']
            },
            {
                name: 'CTA Link Primary',
                element: 'a',
                classes: ['button', 'button--green']
            },
            {
                name: 'CTA Link Secondary',
                element: 'a',
                classes: ['button', 'button--black']
            },
            {
                name: 'Marker',
                element: 'span',
                classes: ['marker']
            },
            {
                name: 'Spoiler',
                element: 'span',
                classes: ['spoiler']
            }
        ]
    },
    table: {
        contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells', 'tableProperties', 'tableCellProperties']
    },
    // Image upload configuration (you might need to customize this based on your backend)
    simpleUpload: {
        uploadUrl: apiUrl, // Replace with your actual upload endpoint
        withCredentials: true,
        headers: {
            'X-CSRF-TOKEN': document.querySelector('meta[name="csrf-token"]')?.getAttribute('content') || '',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        }
    }
};

// Global editor variable
let ckEditor = null;

document.addEventListener('DOMContentLoaded', function () {
    ClassicEditor
        .create(document.querySelector('#editor'), editorConfig)
        .then(editor => {
            ckEditor = editor;


            const wordCount = editor.plugins.get('WordCount');
            const wordCountContainer = document.querySelector('#wordCount');
            const readingTimeContainer = document.querySelector('#readingTime');

            if (wordCountContainer && readingTimeContainer) {
                wordCount.on('update', (evt, stats) => {
                    wordCountContainer.textContent = stats.words;

                    const readingTime = Math.ceil(stats.words / 200);
                    readingTimeContainer.textContent = readingTime + ' min';
                });
            }


            // Get form inputs
            const titleInput = document.querySelector('#Blog_Title');


            let userHasManuallyEditedTitle = false;
            let debounceTimeout;
            const DEBOUNCE_DELAY = 150;
            
            const extractFirstHeading = (htmlContent) => {
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = htmlContent;
                const firstHeading = tempDiv.querySelector('h1, h2, h3, h4, h5, h6');
                return firstHeading ? firstHeading.textContent.trim() : null;
            };

            if (titleInput) {
                titleInput.addEventListener('input', () => {
                    const titleValue = this.value.trim();
                    userHasManuallyEditedTitle = titleValue !== '';
                   
                });
               
            }
            // Listen for changes in the editor content with debouncing
            editor.model.document.on('change:data', () => {
                // Always sync main content immediately
                document.querySelector('#Blog_Content').value = editor.getData();

                // Clear the previous timeout
                clearTimeout(debounceTimeout);

                // Set a new timeout to run the logic after a pause in typing
                debounceTimeout = setTimeout(() => {
                    if (titleInput && !userHasManuallyEditedTitle) {
                        const editorData = editor.getData();
                        const extractedTitle = extractFirstHeading(editorData);

                        if (titleInput.value !== (extractedTitle || '')) {
                            titleInput.value = extractedTitle || '';
                            titleInput.dispatchEvent(new Event('input', { bubbles: true }));
                            titleInput.dispatchEvent(new Event('keyup', { bubbles: true }));
                        }   
                    }
                }, DEBOUNCE_DELAY);
            });

        })
        .catch(error => {
            console.error('Error initializing CKEditor:', error);
        });











    // Main image upload functionality
    const uploadMainImageBtn = document.querySelector('#uploadMainImageBtn');
    const mainImageUpload = document.querySelector('#mainImageUpload');
    const mainImagePreview = document.querySelector('#mainImagePreview');
    const mainImageImg = document.querySelector('#mainImageImg');
    const removeMainImageBtn = document.querySelector('#removeMainImageBtn');
    const mainImageUrlInput = document.querySelector('#Blog_MainImageUrl');
    const mainImageIdInput = document.querySelector('#Blog_MainImageId');

    if (uploadMainImageBtn && mainImageUpload) {
        uploadMainImageBtn.addEventListener('click', () => {
            mainImageUpload.click();
        });

        mainImageUpload.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                // Preview the image
                const reader = new FileReader();
                reader.onload = function (e) {
                    mainImageImg.src = e.target.result;
                    mainImagePreview.style.display = 'block';
                };
                reader.readAsDataURL(file);

            }
        });

        if (removeMainImageBtn) {
            removeMainImageBtn.addEventListener('click', function () {
                mainImagePreview.style.display = 'none';
                mainImageUrlInput.value = '';
                mainImageIdInput.value = '';
                mainImageUpload.value = '';
            });
        }
    }

    // Form submission handling
    const blogForm = document.querySelector('#blogForm');
    const saveBtn = document.querySelector('#saveBtn');
    const saveBtnText = document.querySelector('#saveBtnText');

    if (blogForm) {
        blogForm.addEventListener('submit', function (e) {
            // Ensure editor content is synced before submission
            if (ckEditor) {
                document.querySelector('#Blog_Content').value = ckEditor.getData();
            }

            // Show saving state
            if (saveBtn && saveBtnText) {
                saveBtn.disabled = true;
                saveBtnText.textContent = 'Saving...';
            }
        });
    }
});

