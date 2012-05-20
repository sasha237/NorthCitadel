-- 
-- Структура таблицы `bots`
-- 

CREATE TABLE `bots` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `citizen_id` int(10) unsigned default NULL,
  `login` varchar(255) collate latin1_general_ci NOT NULL,
  `password` varchar(255) collate latin1_general_ci NOT NULL,
  `wellness` float default NULL,
  `happines` float NOT NULL,
  `strength` float default NULL,
  `experience` int(10) default NULL,
  `group` varchar(255) collate latin1_general_ci default NULL,
  `party` varchar(255) collate latin1_general_ci NOT NULL default '',
  `last_day_work` int(10) NOT NULL default '0',
  `last_day_train` int(10) NOT NULL default '0',
  `last_day_study` int(10) NOT NULL,
  `last_day_relax` int(10) NOT NULL,
  `last_day_fight` int(10) NOT NULL default '0',
  `last_day_vote` int(10) NOT NULL default '0',
  `disabled` int(10) NOT NULL default '0',
  `banned` int(10) NOT NULL default '0',
  `industry` int(10) NOT NULL,
  UNIQUE KEY `login` (`login`),
  UNIQUE KEY `id` (`id`),
  KEY `main` (`experience`,`wellness`,`strength`,`login`,`citizen_id`,`disabled`,`banned`),
  KEY `group` (`group`),
  KEY `last_actions` (`id`,`citizen_id`,`last_day_work`,`last_day_train`,`last_day_fight`,`last_day_vote`)
) ENGINE=MyISAM AUTO_INCREMENT=7 DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci AUTO_INCREMENT=7 ;