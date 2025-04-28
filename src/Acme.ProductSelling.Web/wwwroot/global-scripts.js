/* Your Global Scripts */

document.addEventListener('DOMContentLoaded', function () {
    const menuContainer = document.querySelector('.category-dropdown-container');
    if (!menuContainer) return;

    const mainToggleButton = menuContainer.querySelector('#navbarDropdownCategories');
    const mainDropdownMenu = menuContainer.querySelector('.category-hover-menu');

    let mainHideTimeout;
    const HIDE_DELAY = 200; // Milliseconds

    // --- Main Menu Logic ---
    const showMainMenu = () => {
        clearTimeout(mainHideTimeout);
        if (mainDropdownMenu && !mainDropdownMenu.classList.contains('show')) {
            mainDropdownMenu.classList.add('show');
            mainToggleButton.setAttribute('aria-expanded', 'true');
        }
    };

    const startHideMainMenu = () => {
        mainHideTimeout = setTimeout(() => {
            if (mainDropdownMenu && mainDropdownMenu.classList.contains('show')) {
                // Check if the mouse is currently over any submenu before hiding main
                let isHoveringSubmenu = false;
                mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(sm => {
                    if (sm.matches(':hover')) {
                        isHoveringSubmenu = true;
                    }
                });
                 // Also check if hovering the main menu itself
                if (mainDropdownMenu.matches(':hover')) {
                     isHoveringSubmenu = true;
                }

                if (!isHoveringSubmenu) { // Only hide if not hovering a related menu
                    mainDropdownMenu.classList.remove('show');
                    mainToggleButton.setAttribute('aria-expanded', 'false');
                    mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(sm => sm.classList.remove('show'));
                }
            }
        }, HIDE_DELAY);
    };

    menuContainer.addEventListener('mouseenter', showMainMenu);
    menuContainer.addEventListener('mouseleave', startHideMainMenu);

    // --- Submenu (Mega Menu) Logic ---
    if (mainDropdownMenu) {
        const submenuParentItems = mainDropdownMenu.querySelectorAll('.has-submenu');

        submenuParentItems.forEach(item => {
            const submenu = item.querySelector('.submenu'); // Finds the .mega-menu
            let submenuHideTimeout;

            // Function to show this specific submenu and hide others
            const showSubmenu = () => {
                clearTimeout(mainHideTimeout); // Keep main menu open
                clearTimeout(submenuHideTimeout);

                // Hide other currently open sibling submenus
                 mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(openSubmenu => {
                    if (openSubmenu !== submenu) {
                        openSubmenu.classList.remove('show');
                    }
                });

                if (submenu && !submenu.classList.contains('show')) {
                    submenu.classList.add('show');
                }
            }

            // Function to start hiding this submenu
             const startHideSubmenu = () => {
                 if (submenu) {
                     submenuHideTimeout = setTimeout(() => {
                        // Double check: Only hide if mouse isn't back over the trigger item or the submenu itself
                        if (!item.matches(':hover') && !submenu.matches(':hover')) {
                            submenu.classList.remove('show');
                        }
                     }, HIDE_DELAY);
                 }
                  // Also trigger the main menu hide check in case mouse moved completely out
                 startHideMainMenu();
             }

            item.addEventListener('mouseenter', showSubmenu);
            item.addEventListener('mouseleave', startHideSubmenu);

            if (submenu) {
                submenu.addEventListener('mouseenter', () => {
                    clearTimeout(mainHideTimeout);
                    clearTimeout(submenuHideTimeout); // Clear hide timer when entering the submenu
                });

                submenu.addEventListener('mouseleave', startHideSubmenu); // Start hide timer when leaving submenu
            }
        });
    }

    // Click listener for main button (touch fallback)
    mainToggleButton.addEventListener('click', function(event) {
        const isShown = mainDropdownMenu.classList.contains('show');
        if (isShown) {
            mainDropdownMenu.classList.remove('show');
            mainToggleButton.setAttribute('aria-expanded', 'false');
            mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(sm => sm.classList.remove('show'));
        } else {
            mainDropdownMenu.classList.add('show');
            mainToggleButton.setAttribute('aria-expanded', 'true');
        }
    });

    // Click outside to close main menu
    document.addEventListener('click', function(event) {
        if (mainDropdownMenu && !menuContainer.contains(event.target) && mainDropdownMenu.classList.contains('show')) {
             mainDropdownMenu.classList.remove('show');
             mainToggleButton.setAttribute('aria-expanded', 'false');
             mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(sm => sm.classList.remove('show'));
        }
    });
});

