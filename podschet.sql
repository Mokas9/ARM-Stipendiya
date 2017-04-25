-- phpMyAdmin SQL Dump
-- version 4.6.4
-- https://www.phpmyadmin.net/
--
-- Хост: localhost
-- Время создания: Дек 19 2016 г., 22:07
-- Версия сервера: 5.7.16-log
-- Версия PHP: 5.6.25

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `podschet`
--

-- --------------------------------------------------------

--
-- Структура таблицы `faculteti`
--

CREATE TABLE `faculteti` (
  `id_faculteta` int(11) NOT NULL,
  `facultet` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `faculteti`
--

INSERT INTO `faculteti` (`id_faculteta`, `facultet`) VALUES
(1, 'Фак1'),
(2, 'Фак2'),
(3, 'Фак3');

-- --------------------------------------------------------

--
-- Структура таблицы `gruppi`
--

CREATE TABLE `gruppi` (
  `id_gruppi` int(11) NOT NULL,
  `id_specialnosti` int(11) DEFAULT NULL,
  `nomer_gruppi` int(11) DEFAULT NULL,
  `kurs` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `gruppi`
--

INSERT INTO `gruppi` (`id_gruppi`, `id_specialnosti`, `nomer_gruppi`, `kurs`) VALUES
(1, 1, 43244, 3),
(2, 2, 4355, 4);

-- --------------------------------------------------------

--
-- Структура таблицы `kafedri`
--

CREATE TABLE `kafedri` (
  `id_kafedri` int(11) NOT NULL,
  `id_faculteta` int(11) DEFAULT NULL,
  `kafedra` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `kafedri`
--

INSERT INTO `kafedri` (`id_kafedri`, `id_faculteta`, `kafedra`) VALUES
(1, 1, 'Каф1'),
(2, 2, 'Каф2'),
(3, 3, 'Каф3');

-- --------------------------------------------------------

--
-- Структура таблицы `predmeti`
--

CREATE TABLE `predmeti` (
  `id_predmeta` int(11) NOT NULL,
  `predmet` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `predmeti`
--

INSERT INTO `predmeti` (`id_predmeta`, `predmet`) VALUES
(1, 'Физика'),
(2, 'Биология'),
(3, 'Математика'),
(4, 'Химия'),
(5, 'Физкультура'),
(6, 'Русский язык');

-- --------------------------------------------------------

--
-- Структура таблицы `sessii`
--

CREATE TABLE `sessii` (
  `id_sessii` int(11) NOT NULL,
  `id_studenta` int(11) DEFAULT NULL,
  `mark_itog` int(11) DEFAULT NULL,
  `data_sdachi` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `sessii`
--

INSERT INTO `sessii` (`id_sessii`, `id_studenta`, `mark_itog`, `data_sdachi`) VALUES
(7, 1, 10, '23456'),
(8, 2, 8, '65432');

-- --------------------------------------------------------

--
-- Структура таблицы `specialnosti`
--

CREATE TABLE `specialnosti` (
  `id_specialnosti` int(11) NOT NULL,
  `id_kafedri` int(11) DEFAULT NULL,
  `specialnost` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `specialnosti`
--

INSERT INTO `specialnosti` (`id_specialnosti`, `id_kafedri`, `specialnost`) VALUES
(1, 1, 'Спец1'),
(2, 2, 'Спец2');

-- --------------------------------------------------------

--
-- Структура таблицы `stipendiya`
--

CREATE TABLE `stipendiya` (
  `id_stipendii` int(11) NOT NULL,
  `id_studenta` int(11) DEFAULT NULL,
  `summa_stipendii` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `stipendiya`
--

INSERT INTO `stipendiya` (`id_stipendii`, `id_studenta`, `summa_stipendii`) VALUES
(4, 1, '10000'),
(5, 2, '60');

-- --------------------------------------------------------

--
-- Структура таблицы `student`
--

CREATE TABLE `student` (
  `id_studenta` int(11) NOT NULL,
  `id_gruppi` int(11) DEFAULT NULL,
  `familiya` text,
  `imya` text,
  `otchestvo` text,
  `nomer_zach_knijki` int(11) DEFAULT NULL,
  `inogodniy` text,
  `adress_propiski` text,
  `adress_projivaniya` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `student`
--

INSERT INTO `student` (`id_studenta`, `id_gruppi`, `familiya`, `imya`, `otchestvo`, `nomer_zach_knijki`, `inogodniy`, `adress_propiski`, `adress_projivaniya`) VALUES
(1, 1, 'Фам1', 'Им1', 'отч1', 312, 'Да', 'нет', 'нет'),
(2, 2, 'Фам2', 'Им2', 'отч2', 123, 'Нет', 'Да', 'Да');

-- --------------------------------------------------------

--
-- Структура таблицы `zacheti`
--

CREATE TABLE `zacheti` (
  `id_zacheta` int(11) NOT NULL,
  `id_studenta` int(11) DEFAULT NULL,
  `id_predmeta` int(11) DEFAULT NULL,
  `zach_mark` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `zacheti`
--

INSERT INTO `zacheti` (`id_zacheta`, `id_studenta`, `id_predmeta`, `zach_mark`) VALUES
(1, 1, 1, 8),
(2, 1, 2, 9),
(3, 1, 3, 8),
(4, 1, 4, 7),
(5, 2, 6, 7),
(6, 2, 5, 8),
(7, 2, 4, 9),
(8, 2, 1, 10);

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `faculteti`
--
ALTER TABLE `faculteti`
  ADD PRIMARY KEY (`id_faculteta`);

--
-- Индексы таблицы `gruppi`
--
ALTER TABLE `gruppi`
  ADD PRIMARY KEY (`id_gruppi`),
  ADD KEY `gruppi_ibfk_1` (`id_specialnosti`);

--
-- Индексы таблицы `kafedri`
--
ALTER TABLE `kafedri`
  ADD PRIMARY KEY (`id_kafedri`),
  ADD KEY `kafedri_ibfk_1` (`id_faculteta`);

--
-- Индексы таблицы `predmeti`
--
ALTER TABLE `predmeti`
  ADD PRIMARY KEY (`id_predmeta`);

--
-- Индексы таблицы `sessii`
--
ALTER TABLE `sessii`
  ADD PRIMARY KEY (`id_sessii`),
  ADD KEY `sessii_ibfk_1` (`id_studenta`);

--
-- Индексы таблицы `specialnosti`
--
ALTER TABLE `specialnosti`
  ADD PRIMARY KEY (`id_specialnosti`),
  ADD KEY `specialnosti_ibfk_1` (`id_kafedri`);

--
-- Индексы таблицы `stipendiya`
--
ALTER TABLE `stipendiya`
  ADD PRIMARY KEY (`id_stipendii`),
  ADD KEY `stipendiya_ibfk_1` (`id_studenta`);

--
-- Индексы таблицы `student`
--
ALTER TABLE `student`
  ADD PRIMARY KEY (`id_studenta`),
  ADD KEY `student_ibfk_1` (`id_gruppi`);

--
-- Индексы таблицы `zacheti`
--
ALTER TABLE `zacheti`
  ADD PRIMARY KEY (`id_zacheta`),
  ADD KEY `zacheti_ibfk_1` (`id_studenta`),
  ADD KEY `zacheti_ibfk_2` (`id_predmeta`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `faculteti`
--
ALTER TABLE `faculteti`
  MODIFY `id_faculteta` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
--
-- AUTO_INCREMENT для таблицы `gruppi`
--
ALTER TABLE `gruppi`
  MODIFY `id_gruppi` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
--
-- AUTO_INCREMENT для таблицы `kafedri`
--
ALTER TABLE `kafedri`
  MODIFY `id_kafedri` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
--
-- AUTO_INCREMENT для таблицы `predmeti`
--
ALTER TABLE `predmeti`
  MODIFY `id_predmeta` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;
--
-- AUTO_INCREMENT для таблицы `sessii`
--
ALTER TABLE `sessii`
  MODIFY `id_sessii` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;
--
-- AUTO_INCREMENT для таблицы `specialnosti`
--
ALTER TABLE `specialnosti`
  MODIFY `id_specialnosti` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
--
-- AUTO_INCREMENT для таблицы `stipendiya`
--
ALTER TABLE `stipendiya`
  MODIFY `id_stipendii` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
--
-- AUTO_INCREMENT для таблицы `student`
--
ALTER TABLE `student`
  MODIFY `id_studenta` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
--
-- AUTO_INCREMENT для таблицы `zacheti`
--
ALTER TABLE `zacheti`
  MODIFY `id_zacheta` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;
--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `gruppi`
--
ALTER TABLE `gruppi`
  ADD CONSTRAINT `gruppi_ibfk_1` FOREIGN KEY (`id_specialnosti`) REFERENCES `specialnosti` (`id_specialnosti`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `kafedri`
--
ALTER TABLE `kafedri`
  ADD CONSTRAINT `kafedri_ibfk_1` FOREIGN KEY (`id_faculteta`) REFERENCES `faculteti` (`id_faculteta`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `sessii`
--
ALTER TABLE `sessii`
  ADD CONSTRAINT `sessii_ibfk_1` FOREIGN KEY (`id_studenta`) REFERENCES `student` (`id_studenta`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `specialnosti`
--
ALTER TABLE `specialnosti`
  ADD CONSTRAINT `specialnosti_ibfk_1` FOREIGN KEY (`id_kafedri`) REFERENCES `kafedri` (`id_kafedri`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `stipendiya`
--
ALTER TABLE `stipendiya`
  ADD CONSTRAINT `stipendiya_ibfk_1` FOREIGN KEY (`id_studenta`) REFERENCES `student` (`id_studenta`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `student`
--
ALTER TABLE `student`
  ADD CONSTRAINT `student_ibfk_1` FOREIGN KEY (`id_gruppi`) REFERENCES `gruppi` (`id_gruppi`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `zacheti`
--
ALTER TABLE `zacheti`
  ADD CONSTRAINT `zacheti_ibfk_1` FOREIGN KEY (`id_studenta`) REFERENCES `student` (`id_studenta`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `zacheti_ibfk_2` FOREIGN KEY (`id_predmeta`) REFERENCES `predmeti` (`id_predmeta`) ON DELETE CASCADE ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
