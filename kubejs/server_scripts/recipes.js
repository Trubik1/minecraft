// ==============================================================================
//  KubeJS Server Script: Recipes & Mod Progression for Minecraft 1.21.1 (NeoForge)
//  Сборка: Steampunk & Swords (Create + Epic Fight + Refined Storage + Aeronautics)
// ==============================================================================

ServerEvents.recipes(event => {
    console.info('=== Loading Steampunk & Swords Custom Recipes ===');

    // --------------------------------------------------------------------------
    // 2. СВЯЗКА: УЛУЧШЕНИЯ ХРАНИЛИЩ (Storage Drawers & Sophisticated Backpacks x Create)
    // Улучшения ящиков и рюкзаков требуют шестерни и латунные пластины
    // --------------------------------------------------------------------------

    // Медный апгрейд для рюкзака (требует медные пластины Create)
    event.replaceInput(
        { output: 'sophisticatedbackpacks:copper_backpack' },
        'minecraft:copper_ingot',
        'create:copper_sheet'
    );

    // Железный апгрейд для рюкзака (требует железные пластины Create)
    event.replaceInput(
        { output: 'sophisticatedbackpacks:iron_backpack' },
        'minecraft:iron_ingot',
        'create:iron_sheet'
    );

    // Золотой апгрейд для рюкзака (требует золотые пластины Create)
    event.replaceInput(
        { output: 'sophisticatedbackpacks:gold_backpack' },
        'minecraft:gold_ingot',
        'create:golden_sheet'
    );

    // Алмазный апгрейд для рюкзака (требует точный механизм)
    event.replaceInput(
        { output: 'sophisticatedbackpacks:diamond_backpack' },
        'minecraft:diamond',
        'create:precision_mechanism'
    );

    // Улучшения Storage Drawers (требуют латунь и механизмы)
    event.replaceInput(
        { output: 'storagedrawers:obsidian_storage_upgrade' },
        'minecraft:obsidian',
        'create:brass_sheet'
    );

    // --------------------------------------------------------------------------
    // 3. СВЯЗКА: СОЛНЕЧНЫЕ ПАНЕЛИ (Solar Flux Reborn x Create New Age)
    // Фотоэлементы и солнечные панели требуют лазерные кристаллы и медные провода
    // --------------------------------------------------------------------------

    // Базовая панель Solar Flux Tier 1 (требует медный провод Create Crafts & Additions или New Age)
    event.replaceInput(
        { output: 'solarflux:sp_1' },
        'solarflux:mirror',
        'createaddition:copper_wire'
    );

    // --------------------------------------------------------------------------
    // 4. СВЯЗКА: ЦИФРОВЫЕ ТЕХНОЛОГИИ (Refined Storage x Create)
    // Основные крафты корпусов, контроллеров и микросхем находятся в progression_recipes.js
    // --------------------------------------------------------------------------

    // Резка кремния с помощью пилы Slice & Dice
    event.recipes.create.cutting('refinedstorage:silicon', 'minecraft:quartz')
        .id('steampunk:cutting/quartz_silicon');

    // --------------------------------------------------------------------------
    // 5. СВЯЗКА: ДИРИЖАБЛИ И АВИАЦИЯ (Create Aeronautics)
    // Двигатели и штурвалы дирижаблей требуют латунные механизмы и пропеллеры
    // --------------------------------------------------------------------------
    // (Базовые рецепты сбалансированы под латунь и валы Create)

    console.info('=== Steampunk & Swords Recipes Loaded Successfully! ===');
});
