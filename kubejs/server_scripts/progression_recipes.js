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


    // ==========================================================================
    // 11. ТЕХНОЛОГИЧЕСКИЕ И МАГИЧЕСКИЕ СВЯЗКИ (Mekanism x Create x Ars Nouveau)
    // ==========================================================================

    // 1. КОМБАЙНЕР MEKANISM: 100% сборка Точного механизма (БЕЗ РИСКА ПОЛОМКИ И БРАКА!)
    event.custom({
        type: 'mekanism:combining',
        main_input: {
            count: 1,
            item: 'create:brass_casing'
        },
        extra_input: {
            count: 1,
            item: 'create:electron_tube'
        },
        output: {
            count: 1,
            id: 'create:precision_mechanism'
        }
    }).id('steampunk:combining/precision_mechanism_100_percent');

    // 2. КОМБАЙНЕР MEKANISM: Сборка Точного механизма через микросхему Mekanism
    event.custom({
        type: 'mekanism:combining',
        main_input: {
            count: 1,
            item: 'create:brass_sheet'
        },
        extra_input: {
            count: 1,
            item: 'mekanism:basic_control_circuit'
        },
        output: {
            count: 1,
            id: 'create:precision_mechanism'
        }
    }).id('steampunk:combining/precision_mechanism_from_circuit');

    // 3. КАМЕРА ОБОГАЩЕНИЯ: Удвоение сырого цинка Create (x2 переработка руды)
    event.custom({
        type: 'mekanism:enriching',
        input: {
            count: 1,
            item: 'create:raw_zinc'
        },
        output: {
            count: 2,
            id: 'create:zinc_ingot'
        }
    }).id('steampunk:enriching/raw_zinc_doubling');

    // 4. КАМЕРА ОБОГАЩЕНИЯ: Умножение кристаллов маны (Source Gem)
    event.custom({
        type: 'mekanism:enriching',
        input: {
            count: 1,
            item: 'ars_nouveau:source_gem'
        },
        output: {
            count: 2,
            id: 'ars_nouveau:source_gem'
        }
    }).id('steampunk:enriching/source_gem_duplication');

    // 5. НАПОЛНИТЕЛЬ MEKANISM: Прямая штамповка латунных пластин Create из меди
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 10,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:copper_sheet'
        },
        output: {
            count: 2,
            id: 'create:brass_sheet'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/brass_sheet_from_copper');

    // ==========================================================================
    // 12. ЭПИЧЕСКИЙ БОЙ И ОРУЖИЕ БОССОВ (Cataclysm x Epic Fight x Create/Mekanism)
    // ==========================================================================

    // 1. ИНФЕРНАЛЬНЫЙ КИНЕТИЧЕСКИЙ МОЛОТ (Cataclysm Infernal Forge)
    // Объединение молота Монструозности, Парового двигателя Create и стали Mekanism
    event.remove({ output: 'cataclysm:infernal_forge' });
    event.shaped('cataclysm:infernal_forge', [
        'ABA',
        'CDC',
        ' E '
    ], {
        A: 'cataclysm:ignitium_ingot',
        B: 'create:steam_engine',
        C: 'mekanism:ingot_steel',
        D: 'cataclysm:monstrous_horn',
        E: 'epicfight:netherite_greatsword'
    }).id('steampunk:boss_craft/infernal_forge_upgraded');

    // 2. ПЛАЗМЕННЫЙ ИСПЕПЕЛИТЕЛЬ ИГНИСА (Cataclysm The Incinerator)
    // Великий меч Игниса, питаемый энергопланшетом Mekanism и кристаллами Ars Nouveau
    event.remove({ output: 'cataclysm:the_incinerator' });
    event.shaped('cataclysm:the_incinerator', [
        ' A ',
        'BCD',
        ' E '
    ], {
        A: 'cataclysm:ignitium_block',
        B: 'mekanism:energy_tablet',
        C: 'cataclysm:monstrosity_blade',
        D: 'ars_nouveau:source_gem_block',
        E: 'epicfight:netherite_tachi'
    }).id('steampunk:boss_craft/the_incinerator_reforged');

    // 3. КНИГА БОЕВЫХ ИСКУССТВ (Epic Fight Master Skillbook)
    // Ковка фолианта боевых техник (парирование, уклонение, специальные стойки)
    event.shaped('epicfight:skillbook', [
        'ABA',
        'CDC',
        'ABA'
    ], {
        A: 'create:golden_sheet',
        B: 'ars_nouveau:source_gem',
        C: 'mekanism:enriched_alloy',
        D: 'minecraft:book'
    }).id('steampunk:combat/master_skillbook');

    // 4. ТРОФЕЙНАЯ КАТАНА МАСТЕРА (Epic Fight Uchigatana)
    // Из стали Mekanism, Витерита Cataclysm и Точного механизма Create
    event.remove({ output: 'epicfight:uchigatana' });
    event.shaped('epicfight:uchigatana', [
        '  A',
        ' B ',
        'C  '
    ], {
        A: 'cataclysm:witherite_ingot',
        B: 'mekanism:ingot_steel',
        C: 'create:precision_mechanism'
    }).id('steampunk:combat/master_uchigatana');

});
