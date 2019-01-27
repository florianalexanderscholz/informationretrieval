/*
(c) 2019 by Florian Scholz
This file is part of InformationRetrieval.

    InformationRetrieval is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    InformationRetrieval is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with InformationRetrieval.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;

namespace InformationRetrieval.ExpressionParser
{
    /// <summary>
    /// Stores an and Expression for the DNF
    /// </summary>
    public class Conjunction
    {
        /// <summary>
        /// A list of possibly negative variables.
        /// </summary>
        public List<Variable> Variables = new List<Variable>();
    }
}
