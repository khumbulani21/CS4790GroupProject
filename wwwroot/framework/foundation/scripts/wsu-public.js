// JavaScript Document

// Added on 07/31/20 by ech
// check if link has an embeded image, if not addClass or check if parent links are specified, if specified do not addClass

let links = $('a');
const invalidParents = ['.side-nav-desktop', '.tab', '.tabs-title', '.feature-box > .title', '.top-nav'];

for (let i = 0; i < links.length; i++) {
    let link = $(links[i]);
    let hasInvalidParent = false;

    for (let j = 0; j < invalidParents.length; j++) {
        let parents = $(link.parentsUntil(invalidParents[j]));
        let lastItem = $(parents[parents.length - 1]);

        if (!lastItem.is('html')) {
            hasInvalidParent = true;
        }
    }

    if (!hasInvalidParent) {
        // does it not have an image or classes?
        const classList = link.attr('class');
        const hasClasses = (classList === undefined || classList.trim() === '') ? false : true;

        if (!link.find('img').length && !link.find('div').length && !link.find('input').length && !link.find('svg').length && !hasClasses) {
            link.toggleClass('fancy-link');
        }
    }
}

// Added 08/06/2020 by ech, addClass visited to mailto links
$('a[href^="mailto:"]').click(function () {
    $(this).addClass('visited');
});

$(document).ready(function (e) {
    $("#skipToContent").removeClass('fancy-link');
});