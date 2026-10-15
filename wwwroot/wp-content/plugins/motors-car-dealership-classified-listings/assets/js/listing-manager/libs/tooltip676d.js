jQuery(document).ready(function($) {
    const tooltip = $('<div class="mvl-tooltip"></div>');
    const tooltipImage = $('<div class="mvl-tooltip-image"></div>');
    $('body').append(tooltip, tooltipImage);
    const tooltipElement = tooltip[0];
    const tooltipImageElement = tooltipImage[0];

    let showTimeout;
    let hideTimeout;

    function positionTooltip(element, tooltip) {
        const elementRect = element.getBoundingClientRect();
        const tooltipRect = tooltip.getBoundingClientRect();
        const position = element.getAttribute('data-mvl-tooltip-position') || element.getAttribute('mvl-tooltip-position') || 'top';
        
        tooltip.setAttribute('data-position', position);
        
        let top, left;
        const offset = 10;
        
        switch(position) {
            case 'top':
                top = elementRect.top - tooltipRect.height - offset;
                left = elementRect.left + (elementRect.width / 2);
                break;
            case 'bottom':
                top = elementRect.bottom + offset;
                left = elementRect.left + (elementRect.width / 2);
                break;
            case 'left':
                top = elementRect.top + (elementRect.height / 2);
                left = elementRect.left - tooltipRect.width - offset;
                break;
            case 'right':
                top = elementRect.top + (elementRect.height / 2);
                left = elementRect.right + offset;
                break;
        }

        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        if (left < 0) left = 0;
        if (left + tooltipRect.width > viewportWidth) left = viewportWidth - tooltipRect.width;
        if (top < 0) top = 0;
        if (top + tooltipRect.height > viewportHeight) top = viewportHeight - tooltipRect.height;

        tooltip.style.top = (top + window.scrollY) + 'px';
        tooltip.style.left = (left + window.scrollX) + 'px';
    }

    function showTooltip(element, tooltip, content) {
        clearTimeout(hideTimeout);
        clearTimeout(showTimeout);
        
        showTimeout = setTimeout(() => {
            tooltip.textContent = content || '';
            tooltip.style.display = 'block';
            positionTooltip(element, tooltip);
        }, 100);
    }

    function showImageTooltip(element, tooltip, imageUrl) {
        clearTimeout(hideTimeout);
        clearTimeout(showTimeout);

        showTimeout = setTimeout(() => {
            const image = document.createElement('img');
            image.src = imageUrl || '';
            image.alt = 'Preview';

            tooltip.textContent = '';
            tooltip.appendChild(image);
            tooltip.style.display = 'block';
            positionTooltip(element, tooltip);
        }, 100);
    }

    function hideTooltip(tooltip) {
        clearTimeout(showTimeout);
        hideTimeout = setTimeout(() => {
            tooltip.style.display = 'none';
        }, 100);
    }

    $(document).on('mouseenter', '[mvl-tooltip-text], [data-mvl-tooltip-text]', function() {
        const text = this.getAttribute('data-mvl-tooltip-text') || this.getAttribute('mvl-tooltip-text');
        tooltipImageElement.style.display = 'none';
        showTooltip(this, tooltipElement, text);
    });

    $(document).on('mouseleave', '[mvl-tooltip-text], [data-mvl-tooltip-text]', function() {
        hideTooltip(tooltipElement);
    });

    $(document).on('mouseenter', '[mvl-tooltip-image]', function() {
        const imageUrl = this.getAttribute('mvl-tooltip-image');
        tooltipElement.style.display = 'none';
        showImageTooltip(this, tooltipImageElement, imageUrl);
    });

    $(document).on('mouseleave', '[mvl-tooltip-image]', function() {
        hideTooltip(tooltipImageElement);
    });

    $(window).on('scroll resize', function() {
        if (tooltipElement.style.display === 'block') {
            const activeElement = document.querySelector('[mvl-tooltip-text]:hover, [data-mvl-tooltip-text]:hover');
            if (activeElement) {
                positionTooltip(activeElement, tooltipElement);
            }
        }
        if (tooltipImageElement.style.display === 'block') {
            const activeElement = document.querySelector('[mvl-tooltip-image]:hover');
            if (activeElement) {
                positionTooltip(activeElement, tooltipImageElement);
            }
        }
    });
});
