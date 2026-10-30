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
        'SPS',
        'GCG',
        'SPS'
    ], {
        S: 'mekanism:ingot_steel',
        P: ['create:sturdy_sheet', 'industrialforegoing:plastic', 'pneumaticcraft:plastic'],
        G: 'minecraft:glass',
        C: 'create:brass_casing'
    }).id('steampunk:progression/steel_casing');

    // Create: Easy automated rubber & plastic from logs (1 log + water heated = 4 dry rubber -> press = 4 plastic)
    event.recipes.create.compacting('4x industrialforegoing:dryrubber', [
        '#minecraft:logs',
        'minecraft:water_bucket'
    ]).heated().id('steampunk:create/compacting_logs_to_dryrubber');

    event.recipes.create.pressing('industrialforegoing:plastic', 'industrialforegoing:dryrubber')
        .id('steampunk:create/pressing_dryrubber_to_plastic');

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
        A: 'mekanism:alloy_infused',
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
        A: 'mekanism:alloy_infused',
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
        C: 'cataclysm:monstrous_horn',
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
        C: 'mekanism:alloy_infused',
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


    // ==========================================================================
    // 13. ЛЕСТНИЦА ИЗМЕРЕНИЙ (DivineRPG-Style Dimensional Ladder)
    // Покорение миров шаг за шагом: один мир открывает ключ к следующему!
    // ==========================================================================

    // 1. СТУПЕНЬ 2 ➔ 3: КАТАЛИЗАТОР ПОДЗЕМНОГО САДА (Undergarden Catalyst)
    // Требует: Огненный слиток Сумеречного Леса (с Гидры) + Латунь Create + Самоцвет Ars
    event.remove({ output: 'undergarden:catalyst' });
    event.shaped('undergarden:catalyst', [
        'ABA',
        'CDC',
        'AEA'
    ], {
        A: 'minecraft:gold_ingot',
        B: 'twilightforest:fiery_ingot',
        C: 'create:brass_sheet',
        D: 'ars_nouveau:source_gem',
        E: 'minecraft:diamond'
    }).id('steampunk:dimension/undergarden_catalyst');

    // 2. СТУПЕНЬ 3 ➔ 4: СВЯЩЕННЫЙ АЛТАРЬ РАЯ (The Aether Altar)
    // Требует: Забытый слиток Undergarden + Осколок Утровита + Сталь Mekanism + Блок Источника
    event.remove({ output: 'aether:altar' });
    event.shaped('aether:altar', [
        'AAA',
        'BCB',
        'DED'
    ], {
        A: 'aether:holystone',
        B: 'undergarden:utheric_shard',
        C: 'undergarden:forgotten_ingot',
        D: 'mekanism:ingot_steel',
        E: 'ars_nouveau:source_gem_block'
    }).id('steampunk:dimension/aether_altar_gated');

    // 3. СТУПЕНЬ 4 ➔ 5: СЕРДЦЕ ГЛУБИН ДЛЯ МИРА СКАЛКА (Deeper & Darker Heart of The Deep)
    // Требует: Слиток Игнития с босса Cataclysm + Рог Монструозности + Занорит Аэзера + Осколок Эха
    event.remove({ output: 'deeperdarker:heart_of_the_deep' });
    event.shaped('deeperdarker:heart_of_the_deep', [
        'ABA',
        'CDC',
        'AEA'
    ], {
        A: 'minecraft:echo_shard',
        B: 'cataclysm:ignitium_ingot',
        C: 'aether:zanite_gemstone',
        D: 'cataclysm:monstrous_horn',
        E: 'undergarden:forgotten_ingot'
    }).id('steampunk:dimension/heart_of_the_deep_gated');

    // 4. СТУПЕНЬ 5 ➔ 6: ОКО КРАЯ (Eye of Ender)
    // Требует Пыль Душ из измерения The Otherside (Deeper and Darker) для открытия Края!
    event.remove({ output: 'minecraft:ender_eye' });
    event.shaped('minecraft:ender_eye', [
        ' A ',
        'BCB',
        ' A '
    ], {
        A: 'deeperdarker:soul_dust',
        B: 'minecraft:blaze_powder',
        C: 'minecraft:ender_pearl'
    }).id('steampunk:dimension/ender_eye_gated');


    // ==========================================================================
    // СИСТЕМА УЛУЧШЕНИЯ РЮКЗАКОВ (LOLILAND STYLE)
    // Усложненная ступенчатая прогрессия рюкзаков и стаков
    // ==========================================================================

    // 1. Медный рюкзак: Базовый рюкзак + медные листы Create + медь
    event.remove({ output: 'sophisticatedbackpacks:copper_backpack' });
    event.shaped('sophisticatedbackpacks:copper_backpack', [
        'CSC',
        'SBS',
        'CSC'
    ], {
        C: 'minecraft:copper_ingot',
        S: 'create:copper_sheet',
        B: 'sophisticatedbackpacks:backpack'
    }).id('steampunk:backpack/copper_backpack_loliland');

    // 2. Железный рюкзак: Медный рюкзак + железные листы Create + слитки железа
    event.remove({ output: 'sophisticatedbackpacks:iron_backpack' });
    event.shaped('sophisticatedbackpacks:iron_backpack', [
        'ISI',
        'SCS',
        'ISI'
    ], {
        I: 'minecraft:iron_ingot',
        S: 'create:iron_sheet',
        C: 'sophisticatedbackpacks:copper_backpack'
    }).id('steampunk:backpack/iron_backpack_loliland');

    // 3. Золотой рюкзак: Железный рюкзак + золотые листы Create + сплав Mekanism + камень Источника
    event.remove({ output: 'sophisticatedbackpacks:gold_backpack' });
    event.shaped('sophisticatedbackpacks:gold_backpack', [
        'GSG',
        'AIA',
        'GSG'
    ], {
        G: 'minecraft:gold_ingot',
        S: 'create:golden_sheet',
        A: 'mekanism:alloy_infused',
        I: 'sophisticatedbackpacks:iron_backpack'
    }).id('steampunk:backpack/gold_backpack_loliland');

    // 4. Алмазный рюкзак: Золотой рюкзак + точный механизм Create + усиленный сплав Mekanism + алмазы
    event.remove({ output: 'sophisticatedbackpacks:diamond_backpack' });
    event.shaped('sophisticatedbackpacks:diamond_backpack', [
        'DMD',
        'AGA',
        'DMD'
    ], {
        D: 'minecraft:diamond',
        M: 'create:precision_mechanism',
        A: 'mekanism:alloy_reinforced',
        G: 'sophisticatedbackpacks:gold_backpack'
    }).id('steampunk:backpack/diamond_backpack_loliland');

    // 5. Незеритовый рюкзак: Алмазный рюкзак + атомный сплав Mekanism + незеритовые слитки / обломки
    event.remove({ output: 'sophisticatedbackpacks:netherite_backpack' });
    event.shaped('sophisticatedbackpacks:netherite_backpack', [
        'NAN',
        'ADA',
        'NAN'
    ], {
        N: 'minecraft:netherite_ingot',
        A: 'mekanism:alloy_atomic',
        D: 'sophisticatedbackpacks:diamond_backpack'
    }).id('steampunk:backpack/netherite_backpack_loliland');

    // 6. Шаблон улучшения рюкзака (Upgrade Base)
    // Требует латунные листы Create и кожу
    event.remove({ output: 'sophisticatedbackpacks:upgrade_base' });
    event.shaped('sophisticatedbackpacks:upgrade_base', [
        'SLS',
        'LIL',
        'SLS'
    ], {
        S: 'minecraft:string',
        L: 'minecraft:leather',
        I: 'create:brass_sheet'
    }).id('steampunk:backpack/upgrade_base_loliland');

    // 7. Улучшения стаков (Stack Upgrades I - IV)
    event.remove({ output: 'sophisticatedbackpacks:stack_upgrade_tier_1' });
    event.shaped('sophisticatedbackpacks:stack_upgrade_tier_1', [
        'SIS',
        'IUI',
        'SIS'
    ], {
        S: 'create:brass_sheet',
        I: 'minecraft:iron_block',
        U: 'sophisticatedbackpacks:upgrade_base'
    }).id('steampunk:backpack/stack_upgrade_t1_loliland');

    event.remove({ output: 'sophisticatedbackpacks:stack_upgrade_tier_2' });
    event.shaped('sophisticatedbackpacks:stack_upgrade_tier_2', [
        'GAG',
        'AUA',
        'GAG'
    ], {
        G: 'minecraft:gold_block',
        A: 'mekanism:alloy_infused',
        U: 'sophisticatedbackpacks:stack_upgrade_tier_1'
    }).id('steampunk:backpack/stack_upgrade_t2_loliland');

    event.remove({ output: 'sophisticatedbackpacks:stack_upgrade_tier_3' });
    event.shaped('sophisticatedbackpacks:stack_upgrade_tier_3', [
        'DAD',
        'AUA',
        'DAD'
    ], {
        D: 'minecraft:diamond_block',
        A: 'mekanism:alloy_reinforced',
        U: 'sophisticatedbackpacks:stack_upgrade_tier_2'
    }).id('steampunk:backpack/stack_upgrade_t3_loliland');

    event.remove({ output: 'sophisticatedbackpacks:stack_upgrade_tier_4' });
    event.shaped('sophisticatedbackpacks:stack_upgrade_tier_4', [
        'NAN',
        'AUA',
        'NAN'
    ], {
        N: 'minecraft:netherite_ingot',
        A: 'mekanism:alloy_atomic',
        U: 'sophisticatedbackpacks:stack_upgrade_tier_3'
    }).id('steampunk:backpack/stack_upgrade_t4_loliland');

    // ==========================================================================
    // СОЛНЕЧНЫЕ ПАНЕЛИ SOLAR FLUX REBORN (ИНТЕГРАЦИЯ CREATE, MEKANISM, TWILIGHT)
    // ==========================================================================

    // Полное удаление всех стандартных крафтов солнечных панелей Solar Flux Reborn
    event.remove({ id: /^solarflux:(sp_|solar_panel_|sp_tf).*/ });
    event.remove({ id: /^solarflux:solar_panels\/.*/ });
    event.remove({ output: /^solarflux:sp_.*/, not: { id: /^steampunk:solar\/.*/ } });
    event.remove({ output: 'solarflux:sp_tf.fiery', not: { id: /^steampunk:solar\/.*/ } });
    event.remove({ output: 'solarflux:sp_tf.carminite', not: { id: /^steampunk:solar\/.*/ } });

    // Солнечная панель 1: Зеркала + железные листы Create + редстоун
    event.remove({ output: 'solarflux:sp_1' });
    event.shaped('solarflux:sp_1', [
        'MMM',
        'RSR',
        'SSS'
    ], {
        M: 'solarflux:mirror',
        R: 'minecraft:redstone',
        S: 'create:iron_sheet'
    }).id('steampunk:solar/sp_1_craft');

    // Солнечная панель 2: 2x SP 1 + латунные листы Create + Фотоэлемент 1
    event.remove({ output: 'solarflux:sp_2' });
    event.shaped('solarflux:sp_2', [
        'PPP',
        'ECE',
        'ESE'
    ], {
        P: 'solarflux:sp_1',
        C: 'solarflux:photovoltaic_cell_1',
        E: 'powah:steel_energized',
        S: 'powah:capacitor_basic'
    }).id('steampunk:solar/sp_2_craft');

    // Солнечная панель 3: 3x SP 2 + Пылающий кристалл Powah + Закаленный конденсатор + ячейка 2
    event.remove({ output: 'solarflux:sp_3' });
    event.shaped('solarflux:sp_3', [
        'PPP',
        'ECE',
        'AKA'
    ], {
        P: 'solarflux:sp_2',
        C: 'solarflux:photovoltaic_cell_2',
        E: 'powah:crystal_blazing',
        K: 'powah:capacitor_hardened',
        A: 'mekanism:alloy_infused'
    }).id('steampunk:solar/sp_3_craft');

    // Солнечная панель 4: 3x SP 3 + Ниотический кристалл Powah + Пылающий конденсатор + ячейка 3
    event.remove({ output: 'solarflux:sp_4' });
    event.shaped('solarflux:sp_4', [
        'PPP',
        'ECE',
        'NMN'
    ], {
        P: 'solarflux:sp_3',
        C: 'solarflux:photovoltaic_cell_3',
        E: 'powah:crystal_niotic',
        N: 'powah:capacitor_blazing',
        M: 'mekanism:basic_control_circuit'
    }).id('steampunk:solar/sp_4_craft');

    // Солнечная панель 5: 3x SP 4 + Спиритический кристалл Powah + Ниотический конденсатор + ячейка 4
    event.remove({ output: 'solarflux:sp_5' });
    event.shaped('solarflux:sp_5', [
        'PPP',
        'ECE',
        'SMS'
    ], {
        P: 'solarflux:sp_4',
        C: 'solarflux:photovoltaic_cell_4',
        E: 'powah:crystal_spirited',
        S: 'powah:capacitor_niotic',
        M: 'mekanism:advanced_control_circuit'
    }).id('steampunk:solar/sp_5_craft');

    // Солнечная панель 6: 3x SP 5 + Нитро кристалл Powah + Спиритический конденсатор + ячейка 5
    event.remove({ output: 'solarflux:sp_6' });
    event.shaped('solarflux:sp_6', [
        'PPP',
        'ECE',
        'SMS'
    ], {
        P: 'solarflux:sp_5',
        C: 'solarflux:photovoltaic_cell_5',
        E: 'powah:crystal_nitro',
        S: 'powah:capacitor_spirited',
        M: 'mekanism:elite_control_circuit'
    }).id('steampunk:solar/sp_6_craft');

    // Солнечная панель 7: 2x SP 6 + абсолютная схема Mekanism + Звезда Незера
    event.remove({ output: 'solarflux:sp_7' });
    event.shaped('solarflux:sp_7', [
        'PPP',
        'ECE',
        'NSN'
    ], {
        P: 'solarflux:sp_6',
        C: 'solarflux:photovoltaic_cell_6',
        E: 'mekanism:ultimate_control_circuit',
        N: 'minecraft:nether_star',
        S: 'mekanism:alloy_atomic'
    }).id('steampunk:solar/sp_7_craft');

    // Солнечная панель 8 (Максимум): 2x SP 7 + точные механизмы + незеритовые блоки + антиматерия
    event.remove({ output: 'solarflux:sp_8' });
    event.shaped('solarflux:sp_8', [
        'PPP',
        'NAN',
        'MAM'
    ], {
        P: 'solarflux:sp_7',
        N: 'minecraft:netherite_block',
        A: 'mekanism:pellet_antimatter',
        M: 'create:precision_mechanism'
    }).id('steampunk:solar/sp_8_craft');

    // Огненная солнечная панель (Fiery Solar Panel): 4x SP 4 + огненные слитки Twilight Forest + жезлы блейза
    event.remove({ output: 'solarflux:sp_tf.fiery' });
    event.shaped('solarflux:sp_tf.fiery', [
        'FFF',
        'PSP',
        'PBP'
    ], {
        F: 'twilightforest:fiery_ingot',
        P: 'solarflux:sp_4',
        S: 'minecraft:blaze_powder',
        B: 'minecraft:blaze_rod'
    }).id('steampunk:solar/sp_fiery_craft');

    // Карминитовая солнечная панель (Carminite Solar Panel): 4x SP 5 + карминит Twilight Forest
    event.remove({ output: 'solarflux:sp_tf.carminite' });
    event.shaped('solarflux:sp_tf.carminite', [
        'CCC',
        'PSP',
        'PBP'
    ], {
        C: 'twilightforest:carminite',
        P: 'solarflux:sp_5',
        S: 'twilightforest:carminite_block',
        B: 'ars_nouveau:source_gem'
    }).id('steampunk:solar/sp_carminite_craft');

    // ==========================================================================
    // POWAH: ТЕХНО-МАГИЧЕСКАЯ ПРОГРЕССИЯ И КРОСС-МОДОВЫЕ РЕЦЕПТЫ
    // ==========================================================================

    // 1. Металлургический наполнитель: Массовое производство диэлектрической пасты через Углерод
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 20,
            tag: 'mekanism:carbon'
        },
        item_input: {
            count: 1,
            item: 'minecraft:clay_ball'
        },
        output: {
            count: 8,
            id: 'powah:dielectric_paste'
        },
        per_tick_usage: false
    }).id('steampunk:powah/infusing_dielectric_paste');

    // 2. Альтернативный крафт пасты диэлектрика через Create Cinder Flour
    event.shapeless('16x powah:dielectric_paste', [
        'create:cinder_flour',
        'minecraft:clay_ball',
        '#minecraft:coals'
    ]).id('steampunk:powah/dielectric_paste_cinder_flour');

    // 3. Диэлектрические корпуса с повышенным выходом через корпуса Create и Mekanism
    event.shaped('2x powah:dielectric_casing', [
        ' R ',
        'RCR',
        ' R '
    ], {
        R: 'powah:dielectric_rod',
        C: 'create:andesite_casing'
    }).id('steampunk:powah/dielectric_casing_andesite');

    event.shaped('4x powah:dielectric_casing', [
        ' R ',
        'RCR',
        ' R '
    ], {
        R: 'powah:dielectric_rod',
        C: 'create:brass_casing'
    }).id('steampunk:powah/dielectric_casing_brass');

    event.shaped('8x powah:dielectric_casing', [
        ' R ',
        'RCR',
        ' R '
    ], {
        R: 'powah:dielectric_rod',
        C: 'mekanism:steel_casing'
    }).id('steampunk:powah/dielectric_casing_steel');

    // 4. Зарядная сфера (Energizing Orb) — синтез Create, Mekanism и Ars Nouveau
    event.remove({ output: 'powah:energizing_orb' });
    event.shaped('powah:energizing_orb', [
        'RRR',
        'PCE',
        'RSR'
    ], {
        R: 'powah:dielectric_rod',
        P: 'create:precision_mechanism',
        C: 'powah:dielectric_casing',
        E: 'mekanism:basic_control_circuit',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:powah/energizing_orb_progression');

    // 5. Зарядоотводы (Energizing Rods) — ступенчатое технологическое улучшение
    // Базовый стержень: требует латунный лист Create
    event.remove({ output: 'powah:energizing_rod_basic' });
    event.shaped('powah:energizing_rod_basic', [
        ' C ',
        ' B ',
        ' S '
    ], {
        C: 'powah:capacitor_basic',
        B: 'create:brass_sheet',
        S: 'powah:energizing_rod_starter'
    }).id('steampunk:powah/energizing_rod_basic_craft');

    // Закаленный стержень: требует сталь под напряжением + обогащенный сплав Mekanism
    event.remove({ output: 'powah:energizing_rod_hardened' });
    event.shaped('powah:energizing_rod_hardened', [
        ' C ',
        ' A ',
        ' S '
    ], {
        C: 'powah:capacitor_hardened',
        A: 'mekanism:alloy_infused',
        S: 'powah:energizing_rod_basic'
    }).id('steampunk:powah/energizing_rod_hardened_craft');

    // Пылающий стержень: требует пылающий кристалл + армированный сплав Mekanism
    event.remove({ output: 'powah:energizing_rod_blazing' });
    event.shaped('powah:energizing_rod_blazing', [
        ' C ',
        ' A ',
        ' S '
    ], {
        C: 'powah:capacitor_blazing',
        A: 'mekanism:alloy_reinforced',
        S: 'powah:energizing_rod_hardened'
    }).id('steampunk:powah/energizing_rod_blazing_craft');

    // Ниотический стержень: требует ниотический кристалл + слиток очищенного обсидиана
    event.remove({ output: 'powah:energizing_rod_niotic' });
    event.shaped('powah:energizing_rod_niotic', [
        ' C ',
        ' A ',
        ' S '
    ], {
        C: 'powah:capacitor_niotic',
        A: 'mekanism:ingot_refined_obsidian',
        S: 'powah:energizing_rod_blazing'
    }).id('steampunk:powah/energizing_rod_niotic_craft');

    // Резонансный стержень: требует спиритический кристалл + атомарный сплав Mekanism
    event.remove({ output: 'powah:energizing_rod_spirited' });
    event.shaped('powah:energizing_rod_spirited', [
        ' C ',
        ' A ',
        ' S '
    ], {
        C: 'powah:capacitor_spirited',
        A: 'mekanism:alloy_atomic',
        S: 'powah:energizing_rod_niotic'
    }).id('steampunk:powah/energizing_rod_spirited_craft');

    // Нитро стержень: требует нитро-кристалл + точный механизм Create + звезда Незера
    event.remove({ output: 'powah:energizing_rod_nitro' });
    event.shaped('powah:energizing_rod_nitro', [
        ' C ',
        ' N ',
        ' S '
    ], {
        C: 'powah:capacitor_nitro',
        N: 'minecraft:nether_star',
        S: 'powah:energizing_rod_spirited'
    }).id('steampunk:powah/energizing_rod_nitro_craft');

    // 6. Уникальные рецепты Зарядной сферы (Energizing Orb Transmutation)
    // Create Андезитовый сплав + Золото -> 2x Сталь под напряжением
    event.custom({
        type: 'powah:energizing',
        energy: 20000,
        ingredients: [
            { item: 'create:andesite_alloy' },
            { item: 'minecraft:gold_ingot' }
        ],
        result: {
            count: 2,
            id: 'powah:steel_energized'
        }
    }).id('steampunk:powah/energizing_andesite_to_steel');

    // Сумеречный лес Сталелист + Золото -> 2x Сталь под напряжением (быстрая магическая проводка)
    event.custom({
        type: 'powah:energizing',
        energy: 15000,
        ingredients: [
            { item: 'twilightforest:steeleaf_ingot' },
            { item: 'minecraft:gold_ingot' }
        ],
        result: {
            count: 2,
            id: 'powah:steel_energized'
        }
    }).id('steampunk:powah/energizing_steeleaf_to_steel');

    // Mekanism Обогащенный алмаз -> Ниотический кристалл (скидка 50% энергии!)
    event.custom({
        type: 'powah:energizing',
        energy: 150000,
        ingredients: [
            { item: 'mekanism:enriched_diamond' }
        ],
        result: {
            count: 1,
            id: 'powah:crystal_niotic'
        }
    }).id('steampunk:powah/energizing_enriched_diamond_niotic');

    // Ars Nouveau Самоцвет источника + Порошок блейза -> Пылающий кристалл
    event.custom({
        type: 'powah:energizing',
        energy: 60000,
        ingredients: [
            { item: 'ars_nouveau:source_gem' },
            { item: 'minecraft:blaze_powder' }
        ],
        result: {
            count: 1,
            id: 'powah:crystal_blazing'
        }
    }).id('steampunk:powah/energizing_source_gem_blazing');

    // Эндер-ядро: 4x выход при использовании точного механизма Create!
    event.custom({
        type: 'powah:energizing',
        energy: 80000,
        ingredients: [
            { item: 'powah:dielectric_casing' },
            { item: 'minecraft:ender_eye' },
            { item: 'create:precision_mechanism' }
        ],
        result: {
            count: 4,
            id: 'powah:ender_core'
        }
    }).id('steampunk:powah/energizing_ender_core_quad');


    // ==========================================================================
    // ПРОГРЕССИЯ МИКРОСХЕМ MEKANISM: ПРОДВИНУТЫЕ, ЭЛИТНЫЕ (СИНИЕ) И СОВЕРШЕННЫЕ (ФИОЛЕТОВЫЕ)
    // ==========================================================================

    // --------------------------------------------------------------------------
    // 1. ПРОДВИНУТАЯ СХЕМА УПРАВЛЕНИЯ (Advanced Control Circuit - Красная)
    // Синтез: Базовая схема + Прецизионный механизм Create + Обогащенные сплавы + Самоцвет Источника + Полированный розовый кварц
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:advanced_control_circuit' });
    event.shaped('mekanism:advanced_control_circuit', [
        'QMQ',
        'ABA',
        'QSQ'
    ], {
        Q: 'create:polished_rose_quartz',
        M: 'create:precision_mechanism',
        A: 'mekanism:alloy_infused',
        B: 'mekanism:basic_control_circuit',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:circuits/advanced_control_circuit_shaped');

    // Промышленный метод в Наполнителе: Прецизионный механизм + 80 редстоуна
    event.remove({ id: 'mekanism:control_circuit/infused_advanced' });
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 80,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:precision_mechanism'
        },
        output: {
            count: 1,
            id: 'mekanism:advanced_control_circuit'
        },
        per_tick_usage: false
    }).id('steampunk:circuits/advanced_from_precision_infusing');

    // --------------------------------------------------------------------------
    // 2. ЭЛИТНАЯ СХЕМА УПРАВЛЕНИЯ (Elite Control Circuit - СИНЯЯ)
    // Высокотехнологичная плата с охлаждением.
    // Требует: Продвинутая схема + Армированные сплавы + Ниотический кристалл Powah + Сапфировый Занит из Аэзера + Термостойкие листы Create
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:elite_control_circuit' });
    event.remove({ id: 'mekanism:control_circuit/infused_elite' });

    // Основной рецепт: через Небесный мир (The Aether)
    event.shaped('mekanism:elite_control_circuit', [
        'TNT',
        'ACA',
        'TZT'
    ], {
        T: 'create:sturdy_sheet',
        N: 'powah:crystal_niotic',
        A: 'mekanism:alloy_reinforced',
        C: 'mekanism:advanced_control_circuit',
        Z: 'aether:zanite_gemstone'
    }).id('steampunk:circuits/elite_control_circuit_aether');

    // Альтернативный рецепт: через Андергарден (Undergarden Морозная сталь)
    event.shaped('mekanism:elite_control_circuit', [
        'TNT',
        'ACA',
        'TFT'
    ], {
        T: 'create:sturdy_sheet',
        N: 'powah:crystal_niotic',
        A: 'mekanism:alloy_reinforced',
        C: 'mekanism:advanced_control_circuit',
        F: 'undergarden:froststeel_ingot'
    }).id('steampunk:circuits/elite_control_circuit_undergarden');

    // Металлургический наполнитель: Продвинутая схема + 160 ед. Алмазной пыли
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 160,
            tag: 'mekanism:diamond'
        },
        item_input: {
            count: 1,
            item: 'mekanism:advanced_control_circuit'
        },
        output: {
            count: 1,
            id: 'mekanism:elite_control_circuit'
        },
        per_tick_usage: false
    }).id('steampunk:circuits/elite_from_advanced_infusing');

    // --------------------------------------------------------------------------
    // 3. СОВЕРШЕННАЯ СХЕМА УПРАВЛЕНИЯ (Ultimate Control Circuit - ФИОЛЕТОВАЯ)
    // Квантово-магический венец вычислений.
    // Требует: Элитная (синяя) схема + Атомарные сплавы + Нитро-кристалл Powah + Карминит Сумеречного Леса + Очищенный обсидиан
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:ultimate_control_circuit' });
    event.remove({ id: 'mekanism:control_circuit/infused_ultimate' });

    // Основной рецепт: через Карминит Сумеречного Леса и Нитро-кристалл Powah
    event.shaped('mekanism:ultimate_control_circuit', [
        'ONO',
        'AEA',
        'OCO'
    ], {
        O: 'mekanism:ingot_refined_obsidian',
        N: 'powah:crystal_nitro',
        A: 'mekanism:alloy_atomic',
        E: 'mekanism:elite_control_circuit',
        C: 'twilightforest:carminite'
    }).id('steampunk:circuits/ultimate_control_circuit_tf');

    // Альтернативный рецепт: через Зачарованный гравитит Аэзера и Кристалл Утерия Андергардена
    event.shaped('mekanism:ultimate_control_circuit', [
        'GNG',
        'AEA',
        'GUG'
    ], {
        G: 'aether:enchanted_gravitite',
        N: 'powah:crystal_nitro',
        A: 'mekanism:alloy_atomic',
        E: 'mekanism:elite_control_circuit',
        U: 'undergarden:utherium_crystal'
    }).id('steampunk:circuits/ultimate_control_circuit_aether_undergarden');

    // Металлургический наполнитель: Элитная схема + 320 ед. Очищенного обсидиана
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 320,
            tag: 'mekanism:refined_obsidian'
        },
        item_input: {
            count: 1,
            item: 'mekanism:elite_control_circuit'
        },
        output: {
            count: 1,
            id: 'mekanism:ultimate_control_circuit'
        },
        per_tick_usage: false
    }).id('steampunk:circuits/ultimate_from_elite_infusing');

    // --------------------------------------------------------------------------
    // 4. МАССОВАЯ ЗАРЯДКА МИКРОСХЕМ В ЗАРЯДНОЙ СФЕРЕ POWAH (Двойной выход!)
    // --------------------------------------------------------------------------

    // Зарядка Продвинутой схемы (2x выход): Базовая схема + Точный механизм + Самоцвет Источника (100k FE)
    event.custom({
        type: 'powah:energizing',
        energy: 100000,
        ingredients: [
            { item: 'mekanism:basic_control_circuit' },
            { item: 'create:precision_mechanism' },
            { item: 'ars_nouveau:source_gem' }
        ],
        result: {
            count: 2,
            id: 'mekanism:advanced_control_circuit'
        }
    }).id('steampunk:circuits/energizing_advanced_double');

    // Зарядка Элитной схемы (Синей) (2x выход): Продвинутая схема + Ниотический кристалл + Занит (400k FE)
    event.custom({
        type: 'powah:energizing',
        energy: 400000,
        ingredients: [
            { item: 'mekanism:advanced_control_circuit' },
            { item: 'powah:crystal_niotic' },
            { item: 'aether:zanite_gemstone' }
        ],
        result: {
            count: 2,
            id: 'mekanism:elite_control_circuit'
        }
    }).id('steampunk:circuits/energizing_elite_double');

    // Зарядка Совершенной схемы (Фиолетовой) (2x выход): Элитная схема + Нитро-кристалл + Карминит (1.5M FE)
    event.custom({
        type: 'powah:energizing',
        energy: 1500000,
        ingredients: [
            { item: 'mekanism:elite_control_circuit' },
            { item: 'powah:crystal_nitro' },
            { item: 'twilightforest:carminite' }
        ],
        result: {
            count: 2,
            id: 'mekanism:ultimate_control_circuit'
        }
    }).id('steampunk:circuits/energizing_ultimate_double');

    // --------------------------------------------------------------------------
    // 5. REFINED STORAGE: ИНТЕГРАЦИЯ ПРОЦЕССОРОВ С МИКРОСХЕМАМИ MEKANISM
    // --------------------------------------------------------------------------

    // Улучшенный процессор (Синий RS): требует Базовую микросхему Mekanism + Золотой лист Create
    // --- REFINED STORAGE: ГЛУБОКАЯ ИНТЕГРАЦИЯ С POWAH (ТРЕБУЕТСЯ АВТОКРАФТ В ОРБЕ) ---

    // 1. Необработанный базовый процессор: требует Энергетическую сталь Powah
    event.remove({ output: 'refinedstorage:raw_basic_processor' });
    event.shapeless('refinedstorage:raw_basic_processor', [
        'refinedstorage:processor_binding',
        'refinedstorage:silicon',
        'minecraft:redstone',
        'powah:steel_energized'
    ]).id('steampunk:rs/raw_basic_processor_powah');

    // 2. Необработанный улучшенный процессор: требует Пылающий кристалл Powah + базовый микрочип
    event.remove({ output: 'refinedstorage:raw_improved_processor' });
    event.shapeless('refinedstorage:raw_improved_processor', [
        'refinedstorage:processor_binding',
        'refinedstorage:silicon',
        'mekanism:basic_control_circuit',
        'powah:crystal_blazing'
    ]).id('steampunk:rs/raw_improved_processor_powah');

    // 3. Необработанный продвинутый процессор: требует Ниотический кристалл Powah + продвинутый микрочип
    event.remove({ output: 'refinedstorage:raw_advanced_processor' });
    event.shapeless('refinedstorage:raw_advanced_processor', [
        'refinedstorage:processor_binding',
        'refinedstorage:silicon',
        'mekanism:advanced_control_circuit',
        'powah:crystal_niotic'
    ]).id('steampunk:rs/raw_advanced_processor_powah');

    // БОНУСНЫЕ РЕЦЕПТЫ В ENERGIZING ORB (АВТОКРАФТ ДАЕТ x2 ГОТОВЫХ ПРОЦЕССОРА СРАЗУ БЕЗ ПЕРЕПЛАВКИ!)
    // 2x Базовый процессор напрямую из Сферы Зарядки
    event.custom({
        type: 'powah:energizing',
        energy: 15000,
        ingredients: [
            { item: 'refinedstorage:processor_binding' },
            { item: 'refinedstorage:silicon' },
            { item: 'minecraft:redstone' },
            { item: 'minecraft:iron_ingot' }
        ],
        result: {
            count: 2,
            id: 'refinedstorage:basic_processor'
        }
    }).id('steampunk:rs/energizing_basic_processor_double');

    // 2x Улучшенный процессор напрямую из Сферы Зарядки
    event.custom({
        type: 'powah:energizing',
        energy: 35000,
        ingredients: [
            { item: 'refinedstorage:processor_binding' },
            { item: 'refinedstorage:silicon' },
            { item: 'mekanism:basic_control_circuit' },
            { item: 'minecraft:gold_ingot' }
        ],
        result: {
            count: 2,
            id: 'refinedstorage:improved_processor'
        }
    }).id('steampunk:rs/energizing_improved_processor_double');

    // 2x Продвинутый процессор напрямую из Сферы Зарядки
    event.custom({
        type: 'powah:energizing',
        energy: 60000,
        ingredients: [
            { item: 'refinedstorage:processor_binding' },
            { item: 'refinedstorage:silicon' },
            { item: 'mekanism:advanced_control_circuit' },
            { item: 'minecraft:diamond' }
        ],
        result: {
            count: 2,
            id: 'refinedstorage:advanced_processor'
        }
    }).id('steampunk:rs/energizing_advanced_processor_double');

    // Автокрафтер Refined Storage: требует конденсатор Blazing и компоненты Powah
    event.remove({ output: 'refinedstorage:autocrafter' });
    event.shaped('refinedstorage:autocrafter', [
        'ECE',
        'BMB',
        'EDE'
    ], {
        E: 'refinedstorage:quartz_enriched_iron',
        C: 'refinedstorage:construction_core',
        B: 'powah:capacitor_blazing',
        M: 'refinedstorage:machine_casing',
        D: 'refinedstorage:destruction_core'
    }).id('steampunk:rs/autocrafter_powah');

    // Кабельные автокрафтеры Cable Tiers (требуют спиритические и нитро компоненты Powah)
    event.remove({ output: 'cabletiers:elite_autocrafter' });
    event.shaped('cabletiers:elite_autocrafter', [
        'SES',
        'CAC',
        'SES'
    ], {
        S: 'powah:crystal_spirited',
        E: 'powah:capacitor_spirited',
        C: 'refinedstorage:improved_processor',
        A: 'refinedstorage:autocrafter'
    }).id('steampunk:cabletiers/elite_autocrafter_powah');

    event.remove({ output: 'cabletiers:ultra_autocrafter' });
    event.shaped('cabletiers:ultra_autocrafter', [
        'NEN',
        'NAN',
        'NEN'
    ], {
        N: 'powah:crystal_nitro',
        E: 'powah:capacitor_nitro',
        A: 'cabletiers:elite_autocrafter'
    }).id('steampunk:cabletiers/ultra_autocrafter_powah');

    event.remove({ output: 'cabletiers:mega_autocrafter' });
    event.shaped('cabletiers:mega_autocrafter', [
        'NRN',
        'RAR',
        'NRN'
    ], {
        N: 'minecraft:netherite_block',
        R: 'powah:reactor_nitro',
        A: 'cabletiers:ultra_autocrafter'
    }).id('steampunk:cabletiers/mega_autocrafter_powah');


    // ==========================================================================
    // EXTENDED CRAFTING: ЭНДГЕЙМ 9x9 МЕГА-КРАФТЫ (КРЕАТИВНЫЕ ПРЕДМЕТЫ)
    // ==========================================================================

    // 1. Креативный мотор Create (create:creative_motor)
    // NOTE: creative items below now require 1x Niotic Singularity (V) and
    // 1x Ars Singularity (W), both made in the Extended Crafting Compressor.
    event.custom({
        type: 'extendedcrafting:shaped_table',
        tier: 4,
        pattern: [
            'VFPAUAPFM',
            'FSPCHCPFS',
            'PPKOTOKPP',
            'ACONBNOCA',
            'UHTBEBHTU',
            'ACONBNOCA',
            'PPKOTOKPP',
            'FSPCHCPFS',
            'MFPAUAPFW'
        ],
        key: {
            M: { item: 'create:brass_casing' },
            F: { item: 'create:flywheel' },
            P: { item: 'create:precision_mechanism' },
            A: { item: 'mekanism:alloy_atomic' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            S: { item: 'create:speedometer' },
            C: { item: 'twilightforest:carminite' },
            H: { item: 'cataclysm:monstrous_horn' },
            K: { item: 'aether:enchanted_gravitite' },
            O: { item: 'mekanism:ingot_refined_obsidian' },
            T: { item: 'undergarden:forgotten_ingot' },
            N: { item: 'powah:crystal_nitro' },
            B: { item: 'minecraft:nether_star' },
            E: { item: 'minecraft:netherite_block' },
            V: { type: 'neoforge:components', items: ['extendedcrafting:singularity'], components: { 'extendedcrafting:singularity_id': 'extendedcrafting:niotic' }, strict: false },
            W: { type: 'neoforge:components', items: ['extendedcrafting:singularity'], components: { 'extendedcrafting:singularity_id': 'extendedcrafting:ars' }, strict: false }
        },
        result: {
            id: 'create:creative_motor',
            count: 1
        }
    }).id('steampunk:endgame/creative_motor_9x9');

    // 2. Креативный энергетический куб Mekanism (mekanism:creative_energy_cube)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        tier: 4,
        pattern: [
            'VCPUAUPFS',
            'CRUNTNURC',
            'PUMIEIMUP',
            'UNMOBOMNU',
            'ATEBKBETA',
            'UNMOBOMNU',
            'PUMIEIMUP',
            'CRUNTNURC',
            'SFPUAUPFW'
        ],
        key: {
            S: { item: 'solarflux:sp_8' },
            C: { item: 'solarflux:sp_tf.carminite' },
            F: { item: 'solarflux:sp_tf.fiery' },
            R: { item: 'powah:reactor_nitro' },
            P: { item: 'powah:energy_cell_nitro' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            A: { item: 'mekanism:alloy_atomic' },
            N: { item: 'powah:crystal_nitro' },
            T: { item: 'mekanism:pellet_antimatter' },
            I: { item: 'mekanism:ultimate_induction_cell' },
            M: { item: 'mekanism:induction_casing' },
            O: { item: 'mekanism:ingot_refined_obsidian' },
            B: { item: 'minecraft:nether_star' },
            E: { item: 'mekanism:ultimate_energy_cube' },
            K: { item: 'minecraft:netherite_block' },
            V: { type: 'neoforge:components', items: ['extendedcrafting:singularity'], components: { 'extendedcrafting:singularity_id': 'extendedcrafting:niotic' }, strict: false },
            W: { type: 'neoforge:components', items: ['extendedcrafting:singularity'], components: { 'extendedcrafting:singularity_id': 'extendedcrafting:ars' }, strict: false }
        },
        result: {
            id: 'mekanism:creative_energy_cube',
            count: 1
        }
    }).id('steampunk:endgame/creative_energy_cube_9x9');

    // 3. Креативная энергоячейка Powah (powah:energy_cell_creative)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        pattern: [
            'RRRRRRRRR',
            'RPUAIAUPR',
            'RUNTNTNUR',
            'RAIMEIMAR',
            'RUTBEBTUR',
            'RAIMEIMAR',
            'RUNTNTNUR',
            'RPUAIAUPR',
            'RRRRRRRRR'
        ],
        key: {
            R: { item: 'powah:dielectric_casing' },
            P: { item: 'powah:energy_cell_nitro' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            A: { item: 'mekanism:alloy_atomic' },
            I: { item: 'powah:reactor_nitro' },
            N: { item: 'powah:crystal_nitro' },
            T: { item: 'mekanism:pellet_antimatter' },
            M: { item: 'powah:thermo_generator_nitro' },
            E: { item: 'solarflux:sp_8' },
            B: { item: 'minecraft:nether_star' }
        },
        result: {
            id: 'powah:energy_cell_creative',
            count: 1
        }
    }).id('steampunk:endgame/creative_energy_cell_powah_9x9');

    // 4. Креативный контроллер Refined Storage (refinedstorage:creative_controller)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        tier: 4,
        pattern: [
            'VPPPUPPPP',
            'PCSAMASCP',
            'PSOTBTOSP',
            'PAEDRDEAP',
            'UMBKNKBME',
            'PAEDRDEAP',
            'PSOTBTOSP',
            'PCSAMASCP',
            'PPPPUPPPW'
        ],
        key: {
            P: { item: 'refinedstorage:advanced_processor' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            C: { item: 'refinedstorage:controller' },
            S: { item: 'refinedstorage:64k_storage_part' },
            A: { item: 'mekanism:alloy_atomic' },
            M: { item: 'create:precision_mechanism' },
            O: { item: 'mekanism:ingot_refined_obsidian' },
            T: { item: 'ars_nouveau:archmage_spell_book' },
            B: { item: 'minecraft:nether_star' },
            E: { item: 'powah:crystal_nitro' },
            D: { item: 'refinedstorage:machine_casing' },
            R: { item: 'undergarden:forgotten_ingot' },
            K: { item: 'minecraft:netherite_block' },
            N: { item: 'refinedstorage:network_card' },
            V: { type: 'neoforge:components', items: ['extendedcrafting:singularity'], components: { 'extendedcrafting:singularity_id': 'extendedcrafting:niotic' }, strict: false },
            W: { type: 'neoforge:components', items: ['extendedcrafting:singularity'], components: { 'extendedcrafting:singularity_id': 'extendedcrafting:ars' }, strict: false }
        },
        result: {
            id: 'refinedstorage:creative_controller',
            count: 1
        }
    }).id('steampunk:endgame/creative_controller_rs_9x9');

    // 5. Креативный диск памяти Refined Storage (refinedstorage:creative_storage_disk)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        tier: 4,
        pattern: [
            'VSSSUSSSS',
            'SCPAPAPCS',
            'SPOMTMOSP',
            'SAENBNEAP',
            'UATKDKTAU',
            'PAENBNEAP',
            'SPOMTMOSP',
            'SCPAPAPCS',
            'SSSSUSSSW'
        ],
        key: {
            S: { item: 'refinedstorage:64k_storage_part' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            C: { item: 'refinedstorage:creative_controller' },
            P: { item: 'refinedstorage:advanced_processor' },
            A: { item: 'mekanism:alloy_atomic' },
            O: { item: 'mekanism:ingot_refined_obsidian' },
            M: { item: 'create:precision_mechanism' },
            T: { item: 'powah:crystal_nitro' },
            E: { item: 'twilightforest:carminite' },
            N: { item: 'minecraft:nether_star' },
            B: { item: 'undergarden:forgotten_ingot' },
            K: { item: 'refinedstorage:storage_housing' },
            D: { item: 'minecraft:netherite_block' },
            V: { type: 'neoforge:components', items: ['extendedcrafting:singularity'], components: { 'extendedcrafting:singularity_id': 'extendedcrafting:niotic' }, strict: false },
            W: { type: 'neoforge:components', items: ['extendedcrafting:singularity'], components: { 'extendedcrafting:singularity_id': 'extendedcrafting:ars' }, strict: false }
        },
        result: {
            id: 'refinedstorage:creative_storage_disk',
            count: 1
        }
    }).id('steampunk:endgame/creative_storage_disk_9x9');


    // ==========================================================================
    // WAYSTONES: УСЛОЖНЕНИЕ И ИНТЕРТЕХНОЛОГИЧЕСКИЙ КРАФТ ТЕЛЕПОРТАЦИИ
    // ==========================================================================

    // 1. Камень перемещения (Warp Stone) — ядро телепортации
    // Требует: Самоцвет Источника Ars Nouveau + Прецизионный механизм Create + Око Эндера + Аметисты
    event.remove({ output: 'waystones:warp_stone' });
    event.shaped('waystones:warp_stone', [
        'AEA',
        'MSM',
        'AEA'
    ], {
        A: 'minecraft:amethyst_shard',
        E: 'minecraft:ender_eye',
        M: 'create:precision_mechanism',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:waystones/warp_stone_progression');

    // 2. Путевой камень (Waystone)
    // Требует: Камень перемещения + Стальные слитки Mekanism + Каменные кирпичи + Термостойкие пластины Create
    event.remove({ output: 'waystones:waystone' });
    event.shaped('waystones:waystone', [
        ' S ',
        'SWS',
        'TMT'
    ], {
        S: 'minecraft:stone_bricks',
        W: 'waystones:warp_stone',
        T: 'create:sturdy_sheet',
        M: 'mekanism:ingot_steel'
    }).id('steampunk:waystones/waystone_progression');

    // 3. Свиток перемещения (Warp Scroll)
    // Требует: Магический пергамент Ars Nouveau + Пыль варпа + Золото
    event.remove({ output: 'waystones:warp_scroll' });
    event.shaped('3x waystones:warp_scroll', [
        'GDG',
        'GEG',
        'PPP'
    ], {
        G: '#c:nuggets/gold',
        D: 'minecraft:ink_sac',
        E: 'waystones:warp_dust',
        P: 'ars_nouveau:blank_parchment'
    }).id('steampunk:waystones/warp_scroll_progression');

    // 4. Свиток возврата (Return Scroll)
    event.remove({ output: 'waystones:return_scroll' });
    event.shaped('3x waystones:return_scroll', [
        'GEG',
        'PPP'
    ], {
        G: '#c:nuggets/gold',
        E: 'minecraft:ender_pearl',
        P: 'ars_nouveau:blank_parchment'
    }).id('steampunk:waystones/return_scroll_progression');

    // 5. Плита перемещения (Warp Plate)
    // Требует: Латунный корпус Create + Спящий осколок + Пыль варпа + Латунные листы
    event.remove({ output: 'waystones:warp_plate' });
    event.shaped('waystones:warp_plate', [
        'SWS',
        'WFW',
        'SMS'
    ], {
        S: 'create:brass_sheet',
        W: 'waystones:warp_dust',
        F: 'waystones:dormant_shard',
        M: 'create:brass_casing'
    }).id('steampunk:waystones/warp_plate_progression');


    // ==========================================
    // RPG DIMENSION PROGRESSION: UNDERGARDEN CATALYST
    // ==========================================
    // Gated behind Aether exploration (Zanite Gemstone) + Gold + Depthrock / Stone Bricks
    event.remove({ output: 'undergarden:catalyst' });
    event.shaped('undergarden:catalyst', [
        'GCG',
        'ZNZ',
        'GSG'
    ], {
        G: '#c:ingots/gold',
        C: 'minecraft:copper_ingot',
        Z: 'aether:zanite_gemstone',
        N: 'minecraft:ender_pearl',
        S: 'minecraft:stone_bricks'
    }).id('steampunk:undergarden/catalyst_aether_progression');


    // ==========================================
    // 1. CRYSTALTINE INGOT: CREATE SEQUENCED ASSEMBLY
    // ==========================================
    // Sequence: Iron Ingot -> Diamond -> Lapis -> Gold -> Nether Star / Luminessence -> Press
    event.remove({ output: 'extendedcrafting:crystaltine_ingot' });
    
    // Variant A: With Nether Star -> 4x Crystaltine Ingot
    event.recipes.create.sequenced_assembly([
        '4x extendedcrafting:crystaltine_ingot'
    ], 'minecraft:iron_ingot', [
        event.recipes.createDeploying('extendedcrafting:black_iron_ingot', ['extendedcrafting:black_iron_ingot', 'minecraft:diamond']),
        event.recipes.createDeploying('extendedcrafting:black_iron_ingot', ['extendedcrafting:black_iron_ingot', 'minecraft:lapis_lazuli']),
        event.recipes.createDeploying('extendedcrafting:black_iron_ingot', ['extendedcrafting:black_iron_ingot', 'minecraft:gold_ingot']),
        event.recipes.createDeploying('extendedcrafting:black_iron_ingot', ['extendedcrafting:black_iron_ingot', 'minecraft:nether_star']),
        event.recipes.createPressing('extendedcrafting:black_iron_ingot', 'extendedcrafting:black_iron_ingot')
    ]).transitionalItem('extendedcrafting:black_iron_ingot').loops(1).id('steampunk:create/sequenced_crystaltine_star');

// Variant B removed as requested (only Nether Star sequenced assembly remains)

    // ==========================================
    // 2. EXTENDED CRAFTING TABLES PROGRESSION
    // ==========================================
    // Basic Table 3x3
    event.remove({ output: 'extendedcrafting:basic_table' });
    event.shaped('extendedcrafting:basic_table', [
        'BLB',
        'LCL',
        'BSB'
    ], {
        B: 'extendedcrafting:black_iron_ingot',
        L: 'extendedcrafting:luminessence',
        C: 'minecraft:crafting_table',
        S: 'mekanism:block_steel'
    }).id('steampunk:extendedcrafting/basic_table_progression');

    // Advanced Table 5x5 (Requires Industrial Foregoing Advanced Machine Frame)
    event.remove({ output: 'extendedcrafting:advanced_table' });
    event.shaped('extendedcrafting:advanced_table', [
        'CGC',
        'MTM',
        'CBC'
    ], {
        C: 'extendedcrafting:crystaltine_ingot',
        G: 'minecraft:gold_block',
        M: 'industrialforegoing:machine_frame_advanced',
        T: 'extendedcrafting:basic_table',
        B: 'create:brass_casing'
    }).id('steampunk:extendedcrafting/advanced_table_progression');

    // Elite Table 7x7 (Requires Industrial Foregoing Supreme Machine Frame)
    event.remove({ output: 'extendedcrafting:elite_table' });
    event.shaped('extendedcrafting:elite_table', [
        'CDC',
        'MTA',
        'CPC'
    ], {
        C: 'extendedcrafting:crystaltine_block',
        D: 'industrialforegoing:machine_frame_supreme',
        T: 'extendedcrafting:advanced_table',
        M: 'create:precision_mechanism',
        A: 'mekanism:elite_control_circuit',
        P: 'powah:crystal_spirited'
    }).id('steampunk:extendedcrafting/elite_table_progression');

    // Ultimate Table 9x9
    event.remove({ output: 'extendedcrafting:ultimate_table' });
    event.shaped('extendedcrafting:ultimate_table', [
        'CNC',
        'UTE',
        'CQC'
    ], {
        C: 'extendedcrafting:crystaltine_block',
        N: 'minecraft:netherite_block',
        T: 'extendedcrafting:elite_table',
        U: 'mekanism:ultimate_control_circuit',
        E: 'powah:crystal_nitro',
        Q: 'minecraft:nether_star'
    }).id('steampunk:extendedcrafting/ultimate_table_progression');

    // ==========================================
    // 3. THE ULTIMATE INGOT: CREATE MECHANICAL CRAFTING
    // ==========================================
    // 5x5 grid of Mechanical Crafters fusing metals of all dimensions
    event.remove({ output: 'extendedcrafting:the_ultimate_ingot' });
    event.recipes.create.mechanical_crafting('extendedcrafting:the_ultimate_ingot', [
        'ICGZN',
        'BSLMK',
        'XRFUW',
        'OAPTY',
        'VEDHJ'
    ], {
        I: 'minecraft:iron_ingot',
        C: 'minecraft:copper_ingot',
        G: 'minecraft:gold_ingot',
        Z: 'create:zinc_ingot',
        N: 'minecraft:netherite_ingot',
        B: 'create:brass_ingot',
        S: 'mekanism:ingot_steel',
        L: 'mekanism:ingot_lead',
        M: 'mekanism:ingot_osmium',
        K: 'mekanism:ingot_tin',
        X: 'extendedcrafting:black_iron_ingot',
        R: 'extendedcrafting:crystaltine_ingot',
        F: 'extendedcrafting:ultimate_catalyst',
        U: 'undergarden:cloggrum_ingot',
        W: 'undergarden:froststeel_ingot',
        O: 'undergarden:forgotten_ingot',
        A: 'aether:zanite_gemstone',
        P: 'powah:crystal_nitro',
        T: 'cataclysm:ignitium_ingot',
        Y: 'cataclysm:witherite_ingot',
        V: 'minecraft:diamond',
        E: 'minecraft:emerald',
        D: 'minecraft:nether_star',
        H: 'minecraft:echo_shard',
        J: 'minecraft:amethyst_shard'
    }).id('steampunk:create/mechanical_ultimate_ingot');

    // ==========================================
    // 4. MYSTICAL AGRICULTURE SEED PROGRESSION
    // ==========================================
    // Prosperity Seed Base requires Ars Nouveau + Create
    event.remove({ output: 'mysticalagriculture:prosperity_seed_base' });
    event.shaped('mysticalagriculture:prosperity_seed_base', [
        'PSP',
        'SMS',
        'PSP'
    ], {
        P: 'mysticalagriculture:prosperity_shard',
        S: 'ars_nouveau:source_gem',
        M: 'create:precision_mechanism'
    }).id('steampunk:mystical/prosperity_seed_base_progression');

    // Infusion Altar
    event.remove({ output: 'mysticalagriculture:infusion_altar' });
    event.shaped('mysticalagriculture:infusion_altar', [
        'SGS',
        'RCR',
        'SSS'
    ], {
        S: 'mekanism:ingot_steel',
        G: 'minecraft:gold_ingot',
        R: 'minecraft:redstone',
        C: 'ars_nouveau:source_gem'
    }).id('steampunk:mystical/infusion_altar_progression');

    // Infusion Pedestal
    event.remove({ output: 'mysticalagriculture:infusion_pedestal' });
    event.shaped('mysticalagriculture:infusion_pedestal', [
        'S S',
        ' A ',
        'SAS'
    ], {
        S: 'mekanism:ingot_steel',
        A: 'create:andesite_alloy'
    }).id('steampunk:mystical/infusion_pedestal_progression');

    // Infusion Crystal
    event.remove({ output: 'mysticalagriculture:infusion_crystal' });
    event.shaped('mysticalagriculture:infusion_crystal', [
        'PLP',
        'LSL',
        'PLP'
    ], {
        P: 'mysticalagriculture:prosperity_shard',
        L: 'extendedcrafting:luminessence',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:mystical/infusion_crystal_progression');

    // Seed crafts moved exclusively to Create Sequenced Assembly below.


    // ==========================================================================
    // INDUSTRIAL FOREGOING & POWAH ENERGIZING PROGRESSION
    // ==========================================================================

    // Black Iron Ingot via Powah Energizing Orb
    event.remove({ output: 'extendedcrafting:black_iron_ingot' });
    event.custom({
        type: 'powah:energizing',
        energy: 10000,
        ingredients: [
            { item: 'minecraft:iron_ingot' },
            { item: 'minecraft:coal' },
            { item: 'minecraft:coal' },
            { item: 'powah:dielectric_paste' }
        ],
        result: {
            count: 2,
            id: 'extendedcrafting:black_iron_ingot'
        }
    }).id('steampunk:powah/energizing_black_iron_ingot');

    event.custom({
        type: 'powah:energizing',
        energy: 10000,
        ingredients: [
            { item: 'minecraft:iron_ingot' },
            { item: 'minecraft:charcoal' },
            { item: 'minecraft:charcoal' },
            { item: 'powah:dielectric_paste' }
        ],
        result: {
            count: 2,
            id: 'extendedcrafting:black_iron_ingot_charcoal'
        }
    }).id('steampunk:powah/energizing_black_iron_ingot_charcoal');

    // Extended Crafting Core (Requires Industrial Foregoing Advanced Machine Frame)
    event.remove({ output: 'extendedcrafting:crafting_core' });
    event.shaped('extendedcrafting:crafting_core', [
        'BIB',
        'SMS',
        'BIB'
    ], {
        B: 'extendedcrafting:black_iron_ingot',
        I: 'mekanism:alloy_reinforced',
        S: 'minecraft:nether_star',
        M: 'industrialforegoing:machine_frame_advanced'
    }).id('steampunk:extendedcrafting/crafting_core_progression');

    // Powah Reactor Starter (Requires Industrial Foregoing Simple Machine Frame)
    event.remove({ output: 'powah:reactor_starter' });
    event.shaped('powah:reactor_starter', [
        'CDC',
        'DMD',
        'CDC'
    ], {
        C: 'powah:capacitor_basic',
        D: 'powah:dielectric_casing',
        M: 'industrialforegoing:machine_frame_simple'
    }).id('steampunk:powah/reactor_starter_progression');

    // High efficiency Dielectric Casing using Industrial Foregoing Plastic (4x output!)
    event.shaped('4x powah:dielectric_casing', [
        'RPR',
        'PCP',
        'RPR'
    ], {
        R: 'powah:dielectric_rod',
        P: 'industrialforegoing:plastic',
        C: 'create:brass_casing'
    }).id('steampunk:powah/dielectric_casing_plastic');


    // ==========================================================================
    // PIPEZ & FLUX NETWORKS RECIPES
    // ==========================================================================

    // Creative/Infinity Pipe Upgrade on 5x5 Advanced Crafting Table
    event.remove({ output: 'pipez:infinity_upgrade' });
    event.custom({
        type: 'extendedcrafting:shaped_table',
        pattern: [
            'UNNNU',
            'NBIBN',
            'NISIN',
            'NBIBN',
            'UNNNU'
        ],
        key: {
            U: { item: 'pipez:ultimate_upgrade' },
            N: { item: 'minecraft:netherite_ingot' },
            B: { item: 'extendedcrafting:black_iron_ingot' },
            I: { item: 'extendedcrafting:crystaltine_ingot' },
            S: { item: 'minecraft:nether_star' }
        },
        result: { id: 'pipez:infinity_upgrade', count: 1 }
    }).id('steampunk:extendedcrafting/infinity_upgrade_5x5');

    // Flux Dust alternative recipes (crafting table & energizing)
    event.shapeless('2x fluxnetworks:flux_dust', [
        'minecraft:redstone',
        'minecraft:obsidian'
    ]).id('steampunk:fluxnetworks/flux_dust_crafting');

    event.custom({
        type: 'powah:energizing',
        energy: 5000,
        ingredients: [
            { item: 'minecraft:redstone' },
            { item: 'minecraft:obsidian' }
        ],
        result: {
            count: 4,
            id: 'fluxnetworks:flux_dust'
        }
    }).id('steampunk:fluxnetworks/flux_dust_energizing');


    // ==========================================================================
    // MYSTICAL AGRICULTURE: EXCLUSIVE CREATE SEQUENCED ASSEMBLY PROGRESSION
    // Все семена создаются ИСКЛЮЧИТЕЛЬНО через конвейерную сборку Create!
    // Дефолтные рецепты алтаря и верстачные крафты полностью удалены.
    // ==========================================================================

    // 0. Удаляем все дефолтные, верстачные и алтарные рецепты семян
    event.remove({ output: /mysticalagriculture:.*_seeds/ });
    event.remove({ output: /mysticalagradditions:.*_seeds/ });
    event.remove({ type: 'mysticalagriculture:infusion' });

    // 1. Prosperity Seed Base (Конвейерная сборка из пшеничных семян)
    event.remove({ output: 'mysticalagriculture:prosperity_seed_base' });
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:prosperity_seed_base'
    ], 'minecraft:wheat_seeds', [
        event.recipes.createDeploying('minecraft:wheat_seeds', ['minecraft:wheat_seeds', 'mysticalagriculture:prosperity_shard']),
        event.recipes.createDeploying('minecraft:wheat_seeds', ['minecraft:wheat_seeds', 'create:brass_sheet']),
        event.recipes.createDeploying('minecraft:wheat_seeds', ['minecraft:wheat_seeds', 'mysticalagriculture:inferium_essence']),
        event.recipes.createPressing('minecraft:wheat_seeds', 'minecraft:wheat_seeds')
    ]).transitionalItem('minecraft:wheat_seeds').loops(1).id('steampunk:create/sequenced_prosperity_seed_base');

    // 2. Soulium Seed Base (Конвейерная сборка: Prosperity Base + Soulium + Source Gem)
    event.remove({ output: 'mysticalagriculture:soulium_seed_base' });
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:soulium_seed_base'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:soulium_dust']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:soulium_ingot']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'ars_nouveau:source_gem']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_soulium_seed_base');

    // 3. Coal Seeds (Tier 1: Inferium + Coal Block + Andesite Alloy)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:coal_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:coal_block']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:inferium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'create:andesite_alloy']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:inferium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_coal_seeds');

    // 4. Copper Seeds (Tier 2: Prudentium + Copper Block + Zinc Ingot)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:copper_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:copper_block']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prudentium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'create:zinc_ingot']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prudentium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_copper_seeds');

    // 5. Iron Seeds (Tier 3: Tertium + Iron Ingot + Brass Ingot)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:iron_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:iron_ingot']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:tertium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'create:brass_ingot']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:tertium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_iron_seeds');

    // 6. Redstone Seeds (Tier 3: Tertium + Redstone Block + Electron Tube)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:redstone_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:redstone_block']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:tertium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'create:electron_tube']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:tertium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_redstone_seeds');

    // 7. Slime Seeds (Tier 3: Tertium + Slime Block + Super Glue)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:slime_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:slime_block']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:tertium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'create:super_glue']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:tertium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_slime_seeds');

    // 8. Gold Seeds (Tier 4: Imperium + Gold Ingot + Polished Rose Quartz)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:gold_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:gold_ingot']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:imperium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'create:polished_rose_quartz']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:imperium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_gold_seeds');

    // 9. Diamond Seeds (Tier 5: Supremium + Diamond + Reinforced Alloy)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:diamond_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:diamond']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:supremium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mekanism:alloy_reinforced']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:supremium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_diamond_seeds');

    // 10. Netherite Seeds (Tier 5: Supremium + Netherite Ingot + Black Iron Ingot)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:netherite_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:netherite_ingot']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:supremium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'extendedcrafting:black_iron_ingot']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:supremium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_netherite_seeds');

    // 11. Wither Skeleton Seeds (Tier 5 Soulium: Soulium Base + Wither Skeleton Skull + Supremium + Wilden Tribute)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:wither_skeleton_seeds'
    ], 'mysticalagriculture:soulium_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:soulium_seed_base', ['mysticalagriculture:soulium_seed_base', 'minecraft:wither_skeleton_skull']),
        event.recipes.createDeploying('mysticalagriculture:soulium_seed_base', ['mysticalagriculture:soulium_seed_base', 'mysticalagriculture:supremium_essence']),
        event.recipes.createDeploying('mysticalagriculture:soulium_seed_base', ['mysticalagriculture:soulium_seed_base', 'ars_nouveau:wilden_tribute']),
        event.recipes.createDeploying('mysticalagriculture:soulium_seed_base', ['mysticalagriculture:soulium_seed_base', 'mysticalagriculture:supremium_essence']),
        event.recipes.createPressing('mysticalagriculture:soulium_seed_base', 'mysticalagriculture:soulium_seed_base')
    ]).transitionalItem('mysticalagriculture:soulium_seed_base').loops(1).id('steampunk:create/sequenced_wither_skeleton_seeds');

    // 12. Nether Star Seeds (Tier 6: Insanium + Nether Star + Crystaltine Ingot)
    event.recipes.create.sequenced_assembly([
        'mysticalagriculture:nether_star_seeds'
    ], 'mysticalagriculture:prosperity_seed_base', [
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'minecraft:nether_star']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagradditions:insanium_essence']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'extendedcrafting:crystaltine_ingot']),
        event.recipes.createDeploying('mysticalagriculture:prosperity_seed_base', ['mysticalagriculture:prosperity_seed_base', 'mysticalagradditions:insanium_essence']),
        event.recipes.createPressing('mysticalagriculture:prosperity_seed_base', 'mysticalagriculture:prosperity_seed_base')
    ]).transitionalItem('mysticalagriculture:prosperity_seed_base').loops(1).id('steampunk:create/sequenced_nether_star_seeds');

    // ==========================================================================
    // MYSTICAL AGRICULTURE: INFUSION CRYSTALS PROGRESSION
    // ==========================================================================
    // Custom Infusion Crystal (Tier 1) - requires tech/magic cross-integration
    event.remove({ output: 'mysticalagriculture:infusion_crystal' });
    event.shaped('mysticalagriculture:infusion_crystal', [
        'PIP',
        'RDR',
        'PSP'
    ], {
        P: 'mysticalagriculture:prosperity_shard',
        I: 'mysticalagriculture:inferium_essence',
        R: 'create:polished_rose_quartz',
        D: 'minecraft:diamond',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:mysticalagriculture/infusion_crystal');

    // Master Infusion Crystal: Remove standard 3x3 craft
    event.remove({ output: 'mysticalagriculture:master_infusion_crystal' });

    // Master Infusion Crystal - Infusion Altar ritual
    event.custom({
        type: 'mysticalagriculture:infusion',
        input: {
            item: 'mysticalagriculture:infusion_crystal'
        },
        ingredients: [
            { item: 'mysticalagradditions:insanium_essence' },
            { item: 'mysticalagriculture:supremium_gemstone' },
            { item: 'mysticalagradditions:insanium_essence' },
            { item: 'minecraft:nether_star' },
            { item: 'mysticalagradditions:insanium_essence' },
            { item: 'extendedcrafting:crystaltine_ingot' },
            { item: 'mysticalagradditions:insanium_essence' },
            { item: 'mysticalagriculture:supremium_gemstone' }
        ],
        result: {
            id: 'mysticalagriculture:master_infusion_crystal'
        }
    }).id('steampunk:mysticalagriculture/master_infusion_crystal_altar');

    // Master Infusion Crystal - 5x5 Advanced Crafting Table recipe
    event.custom({
        type: 'extendedcrafting:shaped_table',
        pattern: [
            'ISSSI',
            'SGGGS',
            'SGCGS',
            'SGGGS',
            'ISSSI'
        ],
        key: {
            I: { item: 'mysticalagradditions:insanium_essence' },
            S: { item: 'mysticalagriculture:supremium_essence' },
            G: { item: 'mysticalagriculture:supremium_gemstone' },
            C: { item: 'mysticalagriculture:infusion_crystal' }
        },
        result: { id: 'mysticalagriculture:master_infusion_crystal', count: 1 }
    }).id('steampunk:extendedcrafting/master_infusion_crystal_5x5');


    // ==========================================================================
    // ГЛУБОКАЯ ИНТЕГРАЦИЯ POWAH ДЛЯ МАССОВОГО АВТОКРАФТА
    // ==========================================================================

    // --------------------------------------------------------------------------
    // A. АПГРЕЙДЫ MEKANISM (МАССОВОЕ ПРОИЗВОДСТВО: РУЧНОЙ x1 ИЛИ В СФЕРЕ x4)
    // --------------------------------------------------------------------------

    // Апгрейд скорости (Speed Upgrade)
    event.remove({ output: 'mekanism:upgrade_speed' });
    event.shaped('mekanism:upgrade_speed', [
        ' G ',
        'ACA',
        ' O '
    ], {
        G: '#c:glass_blocks',
        A: 'mekanism:alloy_infused',
        C: 'powah:steel_energized',
        O: '#c:dusts/osmium'
    }).id('steampunk:mekanism/upgrade_speed_powah');

    // Опция автокрафта в Сфере Зарядки: дает x4 апгрейда скорости за раз!
    event.custom({
        type: 'powah:energizing',
        energy: 30000,
        ingredients: [
            { item: 'mekanism:alloy_infused' },
            { item: 'mekanism:alloy_infused' },
            { tag: 'c:dusts/osmium' },
            { item: 'powah:steel_energized' },
            { item: 'powah:capacitor_basic' },
            { tag: 'c:glass_blocks' }
        ],
        result: {
            count: 4,
            id: 'mekanism:upgrade_speed'
        }
    }).id('steampunk:mekanism/energizing_speed_upgrade_quad');

    // Апгрейд энергии (Energy Upgrade)
    event.remove({ output: 'mekanism:upgrade_energy' });
    event.shaped('mekanism:upgrade_energy', [
        ' G ',
        'ACA',
        ' D '
    ], {
        G: '#c:glass_blocks',
        A: 'mekanism:alloy_infused',
        C: 'powah:crystal_blazing',
        D: '#c:dusts/gold'
    }).id('steampunk:mekanism/upgrade_energy_powah');

    // Опция автокрафта в Сфере Зарядки: дает x4 апгрейда энергии за раз!
    event.custom({
        type: 'powah:energizing',
        energy: 50000,
        ingredients: [
            { item: 'mekanism:alloy_infused' },
            { item: 'mekanism:alloy_infused' },
            { tag: 'c:dusts/gold' },
            { item: 'powah:crystal_blazing' },
            { item: 'powah:capacitor_hardened' },
            { tag: 'c:glass_blocks' }
        ],
        result: {
            count: 4,
            id: 'mekanism:upgrade_energy'
        }
    }).id('steampunk:mekanism/energizing_energy_upgrade_quad');

    // Апгрейд химикатов (Chemical Upgrade)
    event.remove({ output: 'mekanism:upgrade_chemical' });
    event.shaped('mekanism:upgrade_chemical', [
        ' G ',
        'ACA',
        ' I '
    ], {
        G: '#c:glass_blocks',
        A: 'mekanism:alloy_infused',
        C: 'powah:crystal_niotic',
        I: '#c:dusts/iron'
    }).id('steampunk:mekanism/upgrade_chemical_powah');

    // Опция автокрафта в Сфере Зарядки: дает x4 апгрейда химикатов за раз!
    event.custom({
        type: 'powah:energizing',
        energy: 70000,
        ingredients: [
            { item: 'mekanism:alloy_infused' },
            { item: 'mekanism:alloy_infused' },
            { tag: 'c:dusts/iron' },
            { item: 'powah:crystal_niotic' },
            { item: 'powah:capacitor_blazing' },
            { tag: 'c:glass_blocks' }
        ],
        result: {
            count: 4,
            id: 'mekanism:upgrade_chemical'
        }
    }).id('steampunk:mekanism/energizing_chemical_upgrade_quad');

    // --------------------------------------------------------------------------
    // B. АПГРЕЙДЫ PIPEZ (ЛОГИСТИКА И МАССОВАЯ ТРУБОПРОВОДНАЯ СЕТЬ)
    // --------------------------------------------------------------------------

    event.remove({ output: 'pipez:basic_upgrade' });
    event.shaped('pipez:basic_upgrade', [
        'INI',
        'NPN',
        'INI'
    ], {
        I: '#c:ingots/iron',
        N: '#c:nuggets/iron',
        P: 'powah:steel_energized'
    }).id('steampunk:pipez/basic_upgrade_powah');

    event.remove({ output: 'pipez:improved_upgrade' });
    event.shaped('pipez:improved_upgrade', [
        'GRG',
        'RUR',
        'GRG'
    ], {
        U: 'pipez:basic_upgrade',
        G: '#c:ingots/gold',
        R: 'powah:crystal_blazing'
    }).id('steampunk:pipez/improved_upgrade_powah');

    event.remove({ output: 'pipez:advanced_upgrade' });
    event.shaped('pipez:advanced_upgrade', [
        'DRD',
        'RUR',
        'DRD'
    ], {
        U: 'pipez:improved_upgrade',
        D: '#c:gems/diamond',
        R: 'powah:crystal_niotic'
    }).id('steampunk:pipez/advanced_upgrade_powah');

    event.remove({ output: 'pipez:ultimate_upgrade' });
    event.shaped('pipez:ultimate_upgrade', [
        'NRN',
        'RUR',
        'NRN'
    ], {
        U: 'pipez:advanced_upgrade',
        N: '#c:ingots/netherite',
        R: 'powah:crystal_spirited'
    }).id('steampunk:pipez/ultimate_upgrade_powah');

    // --------------------------------------------------------------------------
    // C. БЕСПРОВОДНАЯ ЭНЕРГОСЕТЬ FLUX NETWORKS (ИНТЕГРАЦИЯ С КРИСТАЛЛАМИ И СФЕРОЙ)
    // --------------------------------------------------------------------------

    // Flux Core: требует Ядро Края Powah (создаваемое в Сфере Зарядки)
    event.remove({ output: 'fluxnetworks:flux_core' });
    event.shaped('4x fluxnetworks:flux_core', [
        'FOF',
        'OEO',
        'FOF'
    ], {
        F: 'fluxnetworks:flux_dust',
        O: 'minecraft:obsidian',
        E: 'powah:ender_core'
    }).id('steampunk:fluxnetworks/flux_core_powah');

    // Flux Plug (Точка отправки энергии): требует Пылающий кристалл Powah
    event.remove({ output: 'fluxnetworks:flux_plug' });
    event.shaped('fluxnetworks:flux_plug', [
        ' C ',
        'CBC',
        ' C '
    ], {
        C: 'fluxnetworks:flux_core',
        B: 'powah:crystal_blazing'
    }).id('steampunk:fluxnetworks/flux_plug_powah');

    // Flux Point (Точка приема энергии): требует Ниотический кристалл Powah
    event.remove({ output: 'fluxnetworks:flux_point' });
    event.shaped('fluxnetworks:flux_point', [
        ' C ',
        'CBC',
        ' C '
    ], {
        C: 'fluxnetworks:flux_core',
        B: 'powah:crystal_niotic'
    }).id('steampunk:fluxnetworks/flux_point_powah');

    // Flux Controller: требует Спиритический кристалл Powah + Flux Блоки
    event.remove({ output: 'fluxnetworks:flux_controller' });
    event.shaped('fluxnetworks:flux_controller', [
        'BCB',
        'FEF',
        'BBB'
    ], {
        B: 'fluxnetworks:flux_block',
        C: 'fluxnetworks:flux_core',
        F: 'fluxnetworks:flux_dust',
        E: 'powah:crystal_spirited'
    }).id('steampunk:fluxnetworks/flux_controller_powah');

    // --------------------------------------------------------------------------
    // D. CREATE CRAFTS & ADDITIONS: ЭЛЕКТРИЧЕСКАЯ ТЕХНИКА
    // --------------------------------------------------------------------------

    // Конденсатор Create: диэлектрическая паста Powah
    event.remove({ output: 'createaddition:capacitor' });
    event.shaped('createaddition:capacitor', [
        'Z',
        'C',
        'P'
    ], {
        Z: '#c:plates/zinc',
        C: '#c:plates/copper',
        P: 'powah:dielectric_paste'
    }).id('steampunk:createaddition/capacitor_powah');

    // Электромотор Create: требует Базовый конденсатор Powah + Энергетическую сталь
    event.remove({ output: 'createaddition:electric_motor' });
    event.custom({
        type: 'create:mechanical_crafting',
        accept_mirrored: true,
        category: 'misc',
        key: {
            A: { item: 'create:andesite_alloy' },
            B: { tag: 'c:plates/brass' },
            C: { item: 'powah:capacitor_basic' },
            R: { item: 'powah:steel_energized' },
            S: { item: 'createaddition:copper_spool' }
        },
        pattern: [
            '  A  ',
            ' BSB ',
            'BSRSB',
            ' BCB '
        ],
        result: {
            count: 1,
            id: 'createaddition:electric_motor'
        }
    }).id('steampunk:createaddition/electric_motor_powah');

    // Альтернатор Create: требует Энергетическую сталь
    event.remove({ output: 'createaddition:alternator' });
    event.custom({
        type: 'create:mechanical_crafting',
        accept_mirrored: true,
        category: 'misc',
        key: {
            A: { item: 'create:andesite_alloy' },
            I: { tag: 'c:plates/iron' },
            R: { item: 'powah:steel_energized' },
            S: { item: 'createaddition:copper_spool' }
        },
        pattern: [
            '  A  ',
            ' ISI ',
            'ISRSI',
            ' ISI ',
            '  A  '
        ],
        result: {
            count: 1,
            id: 'createaddition:alternator'
        }
    }).id('steampunk:createaddition/alternator_powah');

});
