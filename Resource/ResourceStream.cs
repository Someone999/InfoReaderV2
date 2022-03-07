using System;
using System.Collections.Generic;
using System.IO;

namespace InfoReader.Resource;

public class ResourceStream : Stream
{
    public int SegmentLength => 16384;
    private List<byte[]> _segs = new List<byte[]>();
    private long _offset = 0;
    private long _length = 0;
    public ResourceStream(List<byte[]> byteSegments, long length)
    {
        _segs.AddRange(byteSegments);
        _length = length;
    }

    public byte[] GetDataSegment(long pos, int length)
    {
        if (length == 0)
        {
            return Array.Empty<byte>();
        }
        long startSeg = pos / SegmentLength;
        long startPos = pos % SegmentLength;
        long startSize = length;
        long currentOffset = 0;
        startSize = startSize > 16384 ? 16384 - startPos : length;
        byte[] data = new byte[length];
        Array.Copy(_segs[(int) startSeg], startPos, data, 0, startSize);
        currentOffset += startSize;
        while (currentOffset < length)
        {
            startSeg++;
            long currentRest = length - currentOffset;
            long readSize = currentRest > 16384 ? 16384 : currentRest;
            Array.Copy(_segs[(int)startSeg], 0, data, currentOffset, readSize);
            currentOffset += readSize;
        }

        return data;

    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin: 
                _offset = offset;
                break;
            case SeekOrigin.Current: 
                _offset = _offset + offset;
                break;
            case SeekOrigin.End:
                _offset = _length - offset;
                break;
        }
        return offset;
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
       
        if(count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Can not read previous bytes.");
        if (_offset + count > _length - 1)
        {
            count = (int)(_length - _offset);
        }

        byte[] data = GetDataSegment(_offset, count);
        _offset += count;
        Array.Copy(data, 0, buffer, offset, count);
        return count;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("Can not write to a read-only stream.");
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _length;
    public override long Position
    {
        get => _offset;
        set => throw new NotSupportedException("Can not write to a read-only stream.");
    }
}