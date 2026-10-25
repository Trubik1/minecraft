// ==============================================================================
//  KubeJS Server Script: Progression Cross-Mod Recipes for Minecraft 1.21.1
//  Сборка: Steampunk & Swords
//  Связка: Create ➔ Mekanism ➔ Ars Nouveau ➔ Refined Storage (МЭ-Сеть)
// ==============================================================================

ServerEvents.recipes(event => {
    console.info('=== Loading Steampunk & Swords Progression Recipes ===');

    // --------------------------------------------------------------------------
    // 1. MEKANISM: Металлургический наполнитель (Metallurgic Infuser)
    // Входной билет в Mekanism! Требует латунный корпус, пластины и электронную лампу Create.
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:metallurgic_infuser' });
    event.shaped('mekanism:metallurgic_infuser', [
        'SFS',
        'RCR',
        'PTP'
    ], {
        S: 'create:brass_sheet',
        F: 'minecraft:furnace',
        R: 'minecraft:redstone',
        C: 'create:brass_casing',
        P: 'create:copper_sheet',
        T: 'create:electron_tube'
    }).id('steampunk:progression/metallurgic_infuser');

    // --------------------------------------------------------------------------
    // 2. MEKANISM: Стальной корпус (Steel Casing)
    // Основа всех станков Mekanism. Укрепление латунного корпуса Create сталью и стеклом.
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:steel_casing' });
    event.shaped('mekanism:steel_casing', [
        'SGS',
        'GCG',
        'SGS'
    ], {
        S: 'mekanism:ingot_steel',
        G: 'minecraft:glass',
        C: 'create:brass_casing'
    }).id('steampunk:progression/steel_casing');

    // --------------------------------------------------------------------------
    // 3. MEKANISM: Базовая микросхема (Basic Control Circuit)
    // Требует электронную лампу Create, обогащенный сплав, редстоун и латунные дорожки.
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:basic_control_circuit' });
    event.shaped('mekanism:basic_control_circuit', [
        'SRS',
        'ATA',
        'SRS'
    ], {
        S: 'create:brass_sheet',
        R: 'minecraft:redstone',
        A: 'mekanism:enriched_alloy',
        T: 'create:electron_tube'
    }).id('steampunk:progression/basic_control_circuit');

    // --------------------------------------------------------------------------
    // 4. ARS NOUVEAU: Аппарат зачарования (Enchanting Apparatus)
    // Магический алтарь. Требует латунный корпус Create, золотые пластины и самоцветы Источника.
    // --------------------------------------------------------------------------
    event.remove({ output: 'ars_nouveau:enchanting_apparatus' });
    event.shaped('ars_nouveau:enchanting_apparatus', [
        'GSG',
        'SCS',
        'GSG'
    ], {
        G: 'create:golden_sheet',
        S: 'ars_nouveau:source_gem',
        C: 'create:brass_casing'
    }).id('steampunk:progression/enchanting_apparatus');

    // --------------------------------------------------------------------------
    // 5. MEKANISM: Энергетический планшет (Energy Tablet)
    // Аккумулятор высокой емкости на Самоцвете Источника Ars Nouveau и сплавах Mekanism.
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:energy_tablet' });
    event.shaped('mekanism:energy_tablet', [
        'RGR',
        'ASA',
        'RGR'
    ], {
        R: 'minecraft:redstone',
        G: 'create:golden_sheet',
        A: 'mekanism:enriched_alloy',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:progression/energy_tablet');

    // --------------------------------------------------------------------------
    // 6. REFINED STORAGE: Корпус механизма (Machine Casing)
    // Стойка серверов МЭ. Требует стальной корпус Mekanism, стальные слитки и латунь Create.
    // --------------------------------------------------------------------------
    event.remove({ output: 'refinedstorage:machine_casing' });
    event.shaped('refinedstorage:machine_casing', [
        'SBS',
        'BCB',
        'SBS'
    ], {
        S: 'mekanism:ingot_steel',
        B: 'create:brass_sheet',
        C: 'mekanism:steel_casing'
    }).id('steampunk:progression/rs_machine_casing');

    // --------------------------------------------------------------------------
    // 7. REFINED STORAGE: Контроллер МЭ-сети (Controller)
    // Венец технологий! Требует микросхему, энергопланшет, точный механизм Create и самоцвет Ars.
    // --------------------------------------------------------------------------
    event.remove({ output: 'refinedstorage:controller' });
    event.shaped('refinedstorage:controller', [
        'PCP',
        'MET',
        'PSP'
    ], {
        P: 'refinedstorage:advanced_processor',
        C: 'mekanism:basic_control_circuit',
        M: 'create:precision_mechanism',
        E: 'refinedstorage:machine_casing',
        T: 'mekanism:energy_tablet',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:progression/rs_controller');

    // --------------------------------------------------------------------------
    // 8. REFINED STORAGE: Дисковод МЭ-сети (Disk Drive)
    // Требует точный механизм Create, микросхему Mekanism, сундук, сталь и процессор RS.
    // --------------------------------------------------------------------------
    event.remove({ output: 'refinedstorage:disk_drive' });
    event.shaped('refinedstorage:disk_drive', [
        'SPS',
        'CEM',
        'STS'
    ], {
        S: 'mekanism:ingot_steel',
        P: 'refinedstorage:advanced_processor',
        C: 'mekanism:basic_control_circuit',
        E: 'refinedstorage:machine_casing',
        M: 'create:precision_mechanism',
        T: 'minecraft:chest'
    }).id('steampunk:progression/rs_disk_drive');

    // --------------------------------------------------------------------------
    // 9. REFINED STORAGE: Сетка крафта (Crafting Grid)
    // Терминал автоматической сборки. Требует сетку RS, точный механизм Create и самоцвет Ars.
    // --------------------------------------------------------------------------
    event.remove({ output: 'refinedstorage:crafting_grid' });
    event.shaped('refinedstorage:crafting_grid', [
        'BSB',
        'PGM',
        'BTB'
    ], {
        B: 'create:brass_sheet',
        S: 'ars_nouveau:source_gem',
        P: 'refinedstorage:advanced_processor',
        G: 'refinedstorage:grid',
        M: 'create:precision_mechanism',
        T: 'minecraft:crafting_table'
    }).id('steampunk:progression/rs_crafting_grid');

    console.info('=== Steampunk & Swords Progression Recipes Loaded Successfully! ===');

    // --------------------------------------------------------------------------
    // 10. MEKANISM METALLURGIC INFUSER: Электронная лампа Create (Electron Tube)
    // Массовое производство в Металлургическом наполнителе через насыщение редстоуном!
    // --------------------------------------------------------------------------

    // 1. Из железной пластины (create:iron_sheet) + 20 ед. редстоуна
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 20,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:iron_sheet'
        },
        output: {
            count: 1,
            id: 'create:electron_tube'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/electron_tube_from_iron_sheet');

    // 2. Из розового кварца (create:rose_quartz) + 20 ед. редстоуна (без полировки наждачкой!)
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 20,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:rose_quartz'
        },
        output: {
            count: 1,
            id: 'create:electron_tube'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/electron_tube_from_rose_quartz');

    // 3. Из полированного розового кварца (create:polished_rose_quartz) + 10 ед. редстоуна
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 10,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:polished_rose_quartz'
        },
        output: {
            count: 1,
            id: 'create:electron_tube'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/electron_tube_from_polished_quartz');

    // 4. Дополнительно: Ванильная Краснокаменная лампа (minecraft:redstone_lamp) из светящегося камня + 40 редстоуна
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 40,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'minecraft:glowstone'
        },
        output: {
            count: 1,
            id: 'minecraft:redstone_lamp'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/redstone_lamp_from_glowstone');

});
