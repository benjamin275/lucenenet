using Lucene.Net.Support;
using System.Diagnostics;
using System.Text;

namespace Lucene.Net.Search.Spans
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    using ToStringUtils = Lucene.Net.Util.ToStringUtils;

    /// <summary>
    /// Checks to see if the <seealso cref="#getMatch()"/> lies between a start and end position
    /// </summary>
    /// <seealso cref= Lucene.Net.Search.Spans.SpanFirstQuery for a derivation that is optimized for the case where start position is 0 </seealso>
    public class SpanPositionRangeQuery : SpanPositionCheckQuery
    {
        protected int m_start = 0;
        protected int m_end;

        public SpanPositionRangeQuery(SpanQuery match, int start, int end)
            : base(match)
        {
            this.m_start = start;
            this.m_end = end;
        }

        protected override AcceptStatus AcceptPosition(Spans spans)
        {
            Debug.Assert(spans.Start != spans.End);
            if (spans.Start >= m_end)
            {
                return AcceptStatus.NO_AND_ADVANCE;
            }
            else if (spans.Start >= m_start && spans.End <= m_end)
            {
                return AcceptStatus.YES;
            }
            else
            {
                return AcceptStatus.NO;
            }
        }

        /// <returns> The minimum position permitted in a match </returns>
        public virtual int Start
        {
            get
            {
                return m_start;
            }
        }

        /// <returns> the maximum end position permitted in a match. </returns>
        public virtual int End
        {
            get
            {
                return m_end;
            }
        }

        public override string ToString(string field)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("spanPosRange(");
            buffer.Append(m_match.ToString(field));
            buffer.Append(", ").Append(m_start).Append(", ");
            buffer.Append(m_end);
            buffer.Append(")");
            buffer.Append(ToStringUtils.Boost(Boost));
            return buffer.ToString();
        }

        public override object Clone()
        {
            SpanPositionRangeQuery result = new SpanPositionRangeQuery((SpanQuery)m_match.Clone(), m_start, m_end);
            result.Boost = Boost;
            return result;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is SpanPositionRangeQuery))
            {
                return false;
            }

            SpanPositionRangeQuery other = (SpanPositionRangeQuery)o;
            return this.m_end == other.m_end && this.m_start == other.m_start && this.m_match.Equals(other.m_match) && this.Boost == other.Boost;
        }

        public override int GetHashCode()
        {
            int h = m_match.GetHashCode();
            h ^= (h << 8) | ((int)((uint)h >> 25)); // reversible
            h ^= Number.FloatToIntBits(Boost) ^ m_end ^ m_start; // LUCENENET TODO: This was FloatToRawIntBits in the original
            return h;
        }
    }
}